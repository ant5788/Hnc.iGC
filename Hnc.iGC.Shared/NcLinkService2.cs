using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using MQTTnet.Server;

using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hnc.NcLink
{
    public enum HncRegType
    {
        X = 0,
        Y = 1,
        F = 2,
        G = 3,
        R = 4,
        W = 5,
        D = 6,
        B = 7,
        I = 8,
        Q = 9,
        K = 10,
        T = 11,
        C = 12
    }

    public class NcLinkService2
    {
        public NcLinkService2(string server, int? port = 1883)
        {
            ManagedMqttClient = CreateClientAsync(server, port).Result;
        }

        public uint CheckResponseRetryCount { get; set; } = 10;
        public TimeSpan CheckResponseRetryInterval { get; set; } = TimeSpan.FromMilliseconds(100);

        #region MQTT

        private async Task<IManagedMqttClient> CreateClientAsync(string server, int? port = null)
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(server, port)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = false,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true,
                    AllowUntrustedCertificates = true
                })
                .Build())
                .Build();
            var mqttClient = new MqttFactory().CreateManagedMqttClient();
            await mqttClient.StartAsync(options);
            mqttClient.UseApplicationMessageReceivedHandler(HandleReceivedApplicationMessage);
            return mqttClient;
        }

        private readonly List<string> machines = new();

        public IReadOnlyList<string> Machines => new List<string>(machines);

        public async Task AddIfNotExistsAsync(string deviceId)
        {
            if (!machines.Contains(deviceId))
            {
                await ManagedMqttClient.SubscribeAsync(GetTopicResponses(deviceId));
                machines.Add(deviceId);
            }
        }

        public async Task RemoveIfExistsAsync(string deviceId)
        {
            if (machines.Contains(deviceId))
            {
                await ManagedMqttClient.UnsubscribeAsync(GetTopicResponses(deviceId).Select(s => s.Topic));
                machines.Remove(deviceId);
            }
        }

        /// <summary>
        /// Topic -> MessageId -> ResponseMessage
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, QueryResponseMessage>> QueryResponseMessages = new();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, SetResponseMessage>> SetResponseMessages = new();

        private static MqttTopicFilter GetTopic(string topic) => new MqttTopicFilterBuilder().WithTopic(topic).Build();
        private static string GetTopicQueryRequest(string deviceId) => $"Query/Request/{deviceId}";
        private static string GetTopicQueryResponse(string deviceId) => $"Query/Response/{deviceId}";
        private static string GetTopicSetRequest(string deviceId) => $"Set/Request/{deviceId}";
        private static string GetTopicSetResponse(string deviceId) => $"Set/Response/{deviceId}";
        private static IReadOnlyList<MqttTopicFilter> GetTopics(params string[] topics) => topics.Select(s => GetTopic(s)).ToList();
        private static IReadOnlyList<MqttTopicFilter> GetTopicResponses(string deviceId)
            => GetTopics(
                //$"Probe/Query/Response/{deviceId}",
                GetTopicSetResponse(deviceId),
                GetTopicQueryResponse(deviceId));

        /// <summary>
        /// The managed publisher client.
        /// </summary>
        public IManagedMqttClient ManagedMqttClient { get; private set; }

        private void HandleReceivedApplicationMessage(MqttApplicationMessageReceivedEventArgs x)
        {
            var message = x.ApplicationMessage.ConvertPayloadToString();
            if (x.ApplicationMessage.Topic.StartsWith(GetTopicQueryResponse("")))
            {
                var response = JsonSerializer.Deserialize<QueryResponseMessage>(message);
                if (response != null)
                {
                    QueryResponseMessages.AddOrUpdate(
                        x.ApplicationMessage.Topic,
                        k =>
                        {
                            var v = new ConcurrentDictionary<string, QueryResponseMessage>();
                            v.TryAdd(response.MessageId, response);
                            return v;
                        },
                        (k, v) =>
                        {
                            v.TryAdd(response.MessageId, response);
                            return v;
                        });
                }
            }
            else if (x.ApplicationMessage.Topic.StartsWith(GetTopicSetResponse("")))
            {
                var response = JsonSerializer.Deserialize<SetResponseMessage>(message);
                if (response != null)
                {
                    SetResponseMessages.AddOrUpdate(
                        x.ApplicationMessage.Topic,
                        k =>
                        {
                            var v = new ConcurrentDictionary<string, SetResponseMessage>();
                            v.TryAdd(response.MessageId, response);
                            return v;
                        },
                        (k, v) =>
                        {
                            v.TryAdd(response.MessageId, response);
                            return v;
                        });
                }
            }
        }

        private async Task<MqttClientPublishResult> PublishAsync(string topic, string content)
        {
            var payload = Encoding.Default.GetBytes(content);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();
            return await ManagedMqttClient.PublishAsync(message);
        }

        private async Task<T?> GetResponseMessageAsync<T>(string messageId, MqttClientPublishResult pr,
            ConcurrentDictionary<string, ConcurrentDictionary<string, T>> dict, string topic)
        {
            if (pr.ReasonCode != MqttClientPublishReasonCode.Success)
            {
                return default;
            }
            uint times = 0;
            while (true)
            {
                if (dict.TryGetValue(topic, out var responseMessages))
                {
                    if (responseMessages.TryRemove(messageId, out var response))
                    {
                        return response;
                    }
                }

                times++;
                if (times > CheckResponseRetryCount)
                {
                    break;
                }
                else
                {
                    await Task.Delay(CheckResponseRetryInterval);
                }
            }
            return default;
        }

        private async Task<QueryResponseMessage?> GetQueryResponseMessageAsync(string deviceId, string messageId, MqttClientPublishResult pr)
        {
            return await GetResponseMessageAsync(messageId, pr, QueryResponseMessages, GetTopicQueryResponse(deviceId));
        }

        private async Task<SetResponseMessage?> GetSetResponseMessageAsync(string deviceId, string messageId, MqttClientPublishResult pr)
        {
            return await GetResponseMessageAsync(messageId, pr, SetResponseMessages, GetTopicSetResponse(deviceId));
        }

        #endregion

        #region CNC

        private readonly IReadOnlyDictionary<HncRegType, string> RegisterIds = new Dictionary<HncRegType, string>
        {
            { HncRegType.X, "01035430" },
            { HncRegType.Y, "01035431" },
            { HncRegType.F, "01035432" },
            { HncRegType.G, "01035433" },
            { HncRegType.R, "01035434" },
            { HncRegType.W, "01035435" },
            { HncRegType.D, "01035436" },
            { HncRegType.B, "01035437" },
            { HncRegType.I, "01035439" },
            { HncRegType.Q, "01035440" },
            { HncRegType.K, "01035441" },
            { HncRegType.T, "01035442" },
            { HncRegType.C, "01035443" },
        };

        public bool? SetRegister(string deviceId, HncRegType regType, int index, int value)
        {
            return SetRegister(deviceId, regType, (index, value));
        }

        public bool? SetRegister(string deviceId, HncRegType regType, params (int index, int value)[] values)
        {
            return SetRegister(deviceId, regType, values.ToList());
        }

        public bool? SetRegister(string deviceId, HncRegType regType, List<(int index, int value)> values)
        {
            SetRequestMessage requestMessage = new()
            {
                ParamsSet = values.Select(s => (RegisterIds[regType], s.index, (object)s.value)).ToList(),
            };
            var pr = PublishAsync(GetTopicSetRequest(deviceId), requestMessage.ToString()).Result;
            var setResponseMessage = GetSetResponseMessageAsync(deviceId, requestMessage.MessageId, pr).Result;
            return setResponseMessage?.Result.IsSuccessed ?? null;
        }

        public bool? SetRegisterOffset(string deviceId, HncRegType regType, int index, int offset, bool value)
        {
            return SetRegisterOffset(deviceId, regType, (index, offset, value));
        }

        public bool? SetRegisterOffset(string deviceId, HncRegType regType, params (int index, int offset, bool value)[] values)
        {
            return SetRegisterOffset(deviceId, regType, values.ToList());
        }

        public bool? SetRegisterOffset(string deviceId, HncRegType regType, List<(int index, int offset, bool value)> values)
        {
            SetRequestMessage requestMessage = new()
            {
                ParamsSetOffset = values.Select(s => (RegisterIds[regType], s.index, s.offset, (object)(s.value ? 1 : 0))).ToList()
            };
            var pr = PublishAsync(GetTopicSetRequest(deviceId), requestMessage.ToString()).Result;
            var setResponseMessage = GetSetResponseMessageAsync(deviceId, requestMessage.MessageId, pr).Result;
            return setResponseMessage?.Result.IsSuccessed ?? null;
        }

        public uint? GetRegister(string deviceId, HncRegType regType, int index)
        {
            QueryRequestMessage requestMessage = new()
            {
                ParamsGet = new List<(string id, int index)> { (RegisterIds[regType], index) }
            };
            var pr = PublishAsync(GetTopicQueryRequest(deviceId), requestMessage.ToString()).Result;
            var queryResponseMessage = GetQueryResponseMessageAsync(deviceId, requestMessage.MessageId, pr).Result;
            return queryResponseMessage?.FirstValue?.FirstJsonElement?.TryGetUInt32(out uint value) == true ? value : null;
        }

        public IReadOnlyList<uint>? GetRegisters(string deviceId, HncRegType regType, int indexMin, int indexMax)
        {
            QueryRequestMessage requestMessage = new()
            {
                ParamsGets = new List<(string id, int indexMin, int indexMax)> { (RegisterIds[regType], indexMin, indexMax) }
            };
            var pr = PublishAsync(GetTopicQueryRequest(deviceId), requestMessage.ToString()).Result;
            var queryResponseMessage = GetQueryResponseMessageAsync(deviceId, requestMessage.MessageId, pr).Result;
            if (queryResponseMessage?.FirstValue?.IsSuccessed == true)
            {
                var data = new List<uint>();
                foreach (var item in queryResponseMessage.FirstValue.Values)
                {
                    data.Add(item.TryGetUInt32(out uint value) ? value : 0);
                }
                return data;
            }
            return null;
        }

        public bool? GetRegisterOffset(string deviceId, HncRegType regType, int index, int offset)
        {
            var result = GetRegister(deviceId, regType, index);
            return result.HasValue ? ((result.Value >> offset) & 1) == 1 : null;
        }

        public bool? ClearRegister(string deviceId, HncRegType regType, int index)
        {
            return SetRegister(deviceId, regType, index, 0);
        }

        public bool? ClearRegisterOffset(string deviceId, HncRegType regType, int index, int offset)
        {
            return SetRegisterOffset(deviceId, regType, index, offset, false);
        }

        public bool? TriggerRegisterOffset(string deviceId, HncRegType regType, int index, int offset)
        {
            if (SetRegisterOffset(deviceId, regType, index, offset, true) == true)
            {
                Task.Delay(200).Wait();
                return ClearRegisterOffset(deviceId, regType, index, offset);
            }
            return false;
        }

        /// <summary>
        /// 取反
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="regType"></param>
        /// <param name="index"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool? FlipRegister(string deviceId, HncRegType regType, int index, int offset)
        {
            var result = GetRegisterOffset(deviceId, regType, index, offset);
            if (result.HasValue)
            {
                bool value = result.Value;
                value = !value;
                return SetRegisterOffset(deviceId, regType, index, offset, value);
            }
            return false;
        }

        public QueryResponseMessage? Query(string deviceId, string id)
        {
            QueryRequestMessage requestMessage = new()
            {
                IdWithNoParams = new List<string> { id }
            };
            var pr = PublishAsync(GetTopicQueryRequest(deviceId), requestMessage.ToString()).Result;
            return GetQueryResponseMessageAsync(deviceId, requestMessage.MessageId, pr).Result;
        }

        public string? STATUS(string deviceId, string id = "010302")
        {
            return Query(deviceId, id)?.FirstValue?.FirstJsonElement?.GetString() ?? null;
        }

        public double? FEED_SPEED(string deviceId, string id = "010303")
        {
            return Query(deviceId, id)?.FirstValue?.FirstJsonElement?.TryGetDouble(out double value) == true ? value : null;
        }

        public uint? FEED_OVERRIDE(string deviceId, string id = "010305")
        {
            return Query(deviceId, id)?.FirstValue?.FirstJsonElement?.TryGetUInt32(out uint value) == true ? value : null;
        }

        public uint? SPDL_OVERRIDE(string deviceId, string id = "010306")
        {
            return Query(deviceId, id)?.FirstValue?.FirstJsonElement?.TryGetUInt32(out uint value) == true ? value : null;
        }

        public uint? PART_COUNT(string deviceId, string id = "010307")
        {
            return Query(deviceId, id)?.FirstValue?.FirstJsonElement?.TryGetUInt32(out uint value) == true ? value : null;
        }

        public uint? RAPID_OVERRIDE(string deviceId, string id = "010308")
        {
            var a = Query(deviceId, id);
            var b = a?.FirstValue;
            var c = b?.FirstJsonElement;
            return c?.TryGetUInt32(out uint value) == true ? value : null;
        }

        public uint? MAG_TOOL_NO(string deviceId)
        {
            return GetRegister(deviceId, HncRegType.B, 188);
        }

        public uint? SPD_TOOL_NO(string deviceId)
        {
            return GetRegister(deviceId, HncRegType.B, 189);
        }

        private double QueryDouble(string deviceId, string id)
        {
            return Query(deviceId, id)?.FirstValue?.FirstJsonElement?.TryGetDouble(out var value) == true ? value : 0;
        }

        public string Axis(string deviceId)
        {
            var axis0 = new AxisLinear { NAME = "X", NUMBER = 0 };
            axis0.SERVO_DRIVER.POSITION = QueryDouble(deviceId, "0103502001");
            axis0.SERVO_DRIVER.SPEED = QueryDouble(deviceId, "0103502003");
            axis0.MOTOR.CURRENT = QueryDouble(deviceId, "0103502101");
            axis0.SCREW.POSITION = QueryDouble(deviceId, "0103502201");
            axis0.SCREW.SPEED = QueryDouble(deviceId, "0103502202");
            var axis1 = new AxisLinear { NAME = "Y", NUMBER = 1 };
            axis1.SERVO_DRIVER.POSITION = QueryDouble(deviceId, "0103512001");
            axis1.SERVO_DRIVER.SPEED = QueryDouble(deviceId, "0103512003");
            axis1.MOTOR.CURRENT = QueryDouble(deviceId, "0103512101");
            axis1.SCREW.POSITION = QueryDouble(deviceId, "0103512201");
            axis1.SCREW.SPEED = QueryDouble(deviceId, "0103512202");
            var axis2 = new AxisLinear { NAME = "Z", NUMBER = 2 };
            axis2.SERVO_DRIVER.POSITION = QueryDouble(deviceId, "0103522001");
            axis2.SERVO_DRIVER.SPEED = QueryDouble(deviceId, "0103522003");
            axis2.MOTOR.CURRENT = QueryDouble(deviceId, "0103522101");
            axis2.SCREW.POSITION = QueryDouble(deviceId, "0103522201");
            axis2.SCREW.SPEED = QueryDouble(deviceId, "0103522202");
            var axis5 = new AxisRotary { NAME = "C", NUMBER = 5 };
            axis5.SERVO_DRIVER.POSITION = QueryDouble(deviceId, "0103532001");
            axis5.SERVO_DRIVER.SPEED = QueryDouble(deviceId, "0103532002");
            axis5.MOTOR.POSITION = QueryDouble(deviceId, "0103532101");
            axis5.MOTOR.SPEED = QueryDouble(deviceId, "0103532102");
            axis5.MOTOR.CURRENT = QueryDouble(deviceId, "0103532103");
            var axisValue = $"[{axis0},{axis1},{axis2},{axis5}]";
            return axisValue;
        }

        public string? Warning(string deviceId, string id = "01035412")
        {
            return Query(deviceId, id)?.FirstValue?.FirstJsonElement?.ToString() ?? null;
        }

        public double? GetMacroVar(string deviceId, int index, string id = "01035471")
        {
            QueryRequestMessage requestMessage = new()
            {
                ParamsGet = new List<(string id, int index)> { (id, index) }
            };
            var pr = PublishAsync(GetTopicQueryRequest(deviceId), requestMessage.ToString()).Result;
            var queryResponseMessage = GetQueryResponseMessageAsync(deviceId, requestMessage.MessageId, pr).Result;
            return queryResponseMessage?.FirstValue?.FirstJsonElement?.TryGetDouble(out var value) == true ? value : null;
        }

        public bool? SetMacroVar(string deviceId, int index, double value, string id = "01035471")
        {
            SetRequestMessage requestMessage = new()
            {
                ParamsSet = new List<(string id, int index, object value)> { (id, index, value) },
            };
            var pr = PublishAsync(GetTopicSetRequest(deviceId), requestMessage.ToString()).Result;
            var setResponseMessage = GetSetResponseMessageAsync(deviceId, requestMessage.MessageId, pr).Result;
            return setResponseMessage?.Result.IsSuccessed ?? null;
        }

        #endregion
    }

    public class QueryRequestMessage
    {
        [JsonPropertyName("@id")]
        public string MessageId { get; } = Guid.NewGuid().ToString();

        public List<string> IdWithNoParams { get; set; } = new List<string>();

        public List<(string id, int index)> ParamsGet { get; set; } = new List<(string id, int index)>();

        public List<(string id, int indexMin, int indexMax)> ParamsGets { get; set; } = new List<(string id, int indexMin, int indexMax)>();

        public override string ToString()
        {
            var str = "{\"@id\":\"" + MessageId + "\",\"ids\":[";
            foreach (var item in IdWithNoParams)
            {
                str += "{\"id\":\"" + item + "\"},";
            }
            foreach (var (id, index) in ParamsGet)
            {
                str += "{\"id\":\"" + id + "\",\"params\":{\"indexes\":[\"" + index + "\"]}},";
            }
            foreach (var (id, indexMin, indexMax) in ParamsGets)
            {
                str += "{\"id\":\"" + id + "\",\"params\":{\"indexes\":[\"" + indexMin + "-" + indexMax + "\"]}},";
            }
            str = str.Remove(str.Length - 1);
            str += "]}";
            return str;
        }
    }

    public class QueryResponseMessage
    {
        [JsonPropertyName("@id")]
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("values")]
        public List<Body> Values { get; set; } = new List<Body>();

        public Body? FirstValue => Values?.First();

        public class Body
        {
            [JsonPropertyName("code")]
            public string Code { get; set; } = "NG";

            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("values")]
            public List<JsonElement> Values { get; set; } = new List<JsonElement>();

            [JsonIgnore]
            public bool IsSuccessed => Code == "OK";

            [JsonIgnore]
            public JsonElement? FirstJsonElement => Values?.Count > 0 ? (Values?.First().ValueKind == JsonValueKind.Null ? null : Values?.First()) : null;

            public override string ToString() => JsonSerializer.Serialize(this);
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class SetRequestMessage
    {
        [JsonPropertyName("@id")]
        public string MessageId { get; } = Guid.NewGuid().ToString();

        public List<(string id, int index, object value)> ParamsSet { get; set; } = new List<(string id, int index, object value)>();

        public List<(string id, int index, int offset, object value)> ParamsSetOffset { get; set; } = new List<(string id, int index, int offset, object value)>();

        public override string ToString()
        {
            var str = "{\"@id\":\"" + MessageId + "\",\"values\":[";
            foreach ((string id, int index, object value) in ParamsSet)
            {
                str += "{\"id\":\"" + id + "\",\"params\":{\"index\":" + index + ",\"value\":" + value + "}},";
            }
            foreach ((string id, int index, int offset, object value) in ParamsSetOffset)
            {
                str += "{\"id\":\"" + id + "\",\"params\":{\"index\":" + index + ",\"offset\":" + offset + ",\"value\":" + value + "}},";
            }
            str = str.Remove(str.Length - 1);
            str += "]}";
            return str;
        }
    }

    public class SetResponseMessage
    {
        [JsonPropertyName("@id")]
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("results")]
        public List<Body> Results { get; set; } = new List<Body>();

        [JsonIgnore]
        public Body Result => Results?.FirstOrDefault() ?? new Body();

        public class Body
        {
            [JsonPropertyName("code")]
            public string Code { get; set; } = "NG";

            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("params")]
            public JsonElement Params { get; set; }

            [JsonIgnore]
            public bool IsSuccessed => Code == "OK";

            public override string ToString() => JsonSerializer.Serialize(this);
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }


    public class AxisLinear
    {
        public string? NAME { get; set; }
        public int NUMBER { get; set; }
        public string TYPE { get; set; } = "rotary";
        public ServoDriver SERVO_DRIVER { get; set; } = new ServoDriver();
        public Motor MOTOR { get; set; } = new Motor();
        public Screw SCREW { get; set; } = new Screw();

        public override string ToString() => JsonSerializer.Serialize(this);

        public class ServoDriver
        {
            public double POSITION { get; set; }
            public double SPEED { get; set; }
        }

        public class Motor
        {
            public double CURRENT { get; set; }
        }

        public class Screw
        {
            public double POSITION { get; set; }
            public double SPEED { get; set; }
        }

    }

    public class AxisRotary
    {
        public string? NAME { get; set; }
        public int NUMBER { get; set; }
        public string TYPE { get; set; } = "rotary";
        public ServoDriver SERVO_DRIVER { get; set; } = new ServoDriver();
        public Motor MOTOR { get; set; } = new Motor();

        public override string ToString() => JsonSerializer.Serialize(this);

        public class ServoDriver
        {
            public double POSITION { get; set; }
            public double SPEED { get; set; }
        }

        public class Motor
        {
            public double POSITION { get; set; }
            public double SPEED { get; set; }
            public double CURRENT { get; set; }
        }

    }

}
