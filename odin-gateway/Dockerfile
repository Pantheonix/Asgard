FROM envoyproxy/envoy:v1.14.2

# COPY ./https/https.crt /etc/ssl/certs/https.crt
# COPY ./https/key.pem /etc/ssl/certs/key.pem

COPY envoy.yaml /etc/envoy/envoy.yaml

# RUN apt-get update && apt-get install -y gettext-base
# RUN cat /tmpl/envoy.yaml.tmpl | envsubst \$ENVOY_ADMIN_PORT,\$ENVOY_LISTENER_0_PORT,\$ENVOY_LISTENER_1_PORT,\$DAPR_SIDECAR_PORT,\$QUETZALCOATL_AUTH_API_PORT,\$ENKI_PROBLEMS_API_PORT,\$ANUBIS_EVAL_API_PORT > /etc/envoy/envoy.yaml

CMD /usr/local/bin/envoy -c /etc/envoy/envoy.yaml --service-cluster odin-gateway --service-node odin-gateway --log-level debug