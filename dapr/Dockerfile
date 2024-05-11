FROM tianon/true

COPY components/statestore.yaml /components/statestore.yaml
COPY components/pubsub.yaml /components/pubsub.yaml
COPY components/problem_eval_metadata_upserted_subscription.yaml /components/problem_eval_metadata_upserted_subscription.yaml

COPY config/config.yaml /config/config.yaml

VOLUME /components
VOLUME /config