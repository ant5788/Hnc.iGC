﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hnc.iGC.Models
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeSpan.TryParse(reader.GetString(), out var r) ? r : TimeSpan.Zero;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
