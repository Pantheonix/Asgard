FROM envoyproxy/envoy:v1.14.2

COPY envoy.yaml /etc/envoy/envoy.yaml

CMD /usr/local/bin/envoy -c /etc/envoy/envoy.yaml --service-cluster odin-gateway --service-node odin-gateway --log-level debug