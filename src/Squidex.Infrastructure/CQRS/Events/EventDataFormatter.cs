﻿// ==========================================================================
//  EventDataFormatter.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using Newtonsoft.Json;
// ReSharper disable InconsistentNaming

namespace Squidex.Infrastructure.CQRS.Events
{
    public class EventDataFormatter
    {
        private readonly JsonSerializerSettings serializerSettings;

        public EventDataFormatter(JsonSerializerSettings serializerSettings = null)
        {
            this.serializerSettings = serializerSettings ?? new JsonSerializerSettings();
        }

        public virtual Envelope<IEvent> Parse(EventData eventData)
        {
            var headers = ReadJson<PropertiesBag>(eventData.Metadata);

            var eventType = TypeNameRegistry.GetType(eventData.Type);
            var eventContent = ReadJson<IEvent>(eventData.Payload, eventType);

            var envelope = new Envelope<IEvent>(eventContent, headers);

            return envelope;
        }

        public virtual EventData ToEventData(Envelope<IEvent> envelope, Guid commitId)
        {
            var eventType = TypeNameRegistry.GetName(envelope.Payload.GetType());

            envelope.SetCommitId(commitId);

            var headers = WriteJson(envelope.Headers);
            var content = WriteJson(envelope.Payload);

            return new EventData { EventId = envelope.Headers.EventId(), Type = eventType, Payload = content, Metadata = headers };
        }

        private T ReadJson<T>(string data, Type type = null)
        {
            return (T)JsonConvert.DeserializeObject(data, type ?? typeof(T), serializerSettings);
        }

        private string WriteJson(object value)
        {
            return JsonConvert.SerializeObject(value, serializerSettings);
        }
    }
}
