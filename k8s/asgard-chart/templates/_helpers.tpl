{{- define "odin.image" -}}
{{- printf "%s/%s:%s" .Values.microservices.odin.image.repository .Values.microservices.odin.image.name .Values.microservices.odin.image.tag -}}
{{- end -}}

{{- define "quetzalcoatl.image" -}}
{{- printf "%s/%s:%s" .Values.microservices.quetzalcoatl.image.repository .Values.microservices.quetzalcoatl.image.name .Values.microservices.quetzalcoatl.image.tag -}}
{{- end -}}

{{- define "enki.image" -}}
{{- printf "%s/%s:%s" .Values.microservices.enki.image.repository .Values.microservices.enki.image.name .Values.microservices.enki.image.tag -}}
{{- end -}}

{{- define "hermes.image" }}
{{- printf "%s/%s:%s" .Values.microservices.hermes.image.repository .Values.microservices.hermes.image.name .Values.microservices.hermes.image.tag -}}
{{- end -}}

{{- define "anubis.image" }}
{{- printf "%s/%s:%s" .Values.microservices.anubis.image.repository .Values.microservices.anubis.image.name .Values.microservices.anubis.image.tag -}}
{{- end -}}

{{- define "judge0.image" -}}
{{- printf "%s/%s:%s" .Values.microservices.judge0.image.repository .Values.microservices.judge0.image.name .Values.microservices.judge0.image.tag -}}
{{- end -}}

{{- define "judge0.redis.fullname" -}}
{{- printf "%s-redis-svc" .Values.microservices.judge0.name -}}
{{- end -}}

{{- define "judge0.postgresql.fullname" -}}
{{- printf "%s-db-svc" .Values.microservices.judge0.name -}}
{{- end -}}