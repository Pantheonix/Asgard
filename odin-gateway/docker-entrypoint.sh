#!/bin/sh
set -e

echo "Generating envoy.yaml config file..."
cat /tmpl/envoy.yaml.tmpl | envsubst \$ENVOY_ADMIN_PORT,\$ENVOY_HTTP_PORT,\$ENVOY_HTTPS_PORT,\$DAPR_SIDECAR_PORT,\$QUETZALCOATL_AUTH_API_PORT,\$ENKI_PROBLEMS_API_PORT,\$ANUBIS_EVAL_API_PORT > /etc/envoy.yaml

echo "Starting Envoy..."
/usr/local/bin/envoy -c /etc/envoy.yaml