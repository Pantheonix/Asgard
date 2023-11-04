FROM tianon/true

COPY components/statestore.yaml /components/statestore.yaml
# COPY components/secrets-file.yaml.compose /components/secrets-file.yaml
# COPY components/pubsub.yaml /components
# COPY components/secrets-envvars.yaml /components
# COPY components/secrets-keyvault.yaml /components
# COPY components/secrets.json /components

COPY config/config.yaml.compose /config/config.yaml

VOLUME /components
VOLUME /config