FROM hashicorp/consul-template:0.39.1

ENV CONSUL_ADDR="localhost:8500"

ENV TEMPLATE_DIR="/templates/"
ENV TEMPLATE_FILE="haproxy.ctmpl"
ENV OUTPUT_FILE="/shared/haproxy.cfg"
ENV RELOAD_CMD="docker kill -s HUP haproxy"

USER 0

RUN apk update &&  \
    apk add --no-cache docker-cli

RUN OUTPUT_DIR=$(dirname "$OUTPUT_FILE") &&  \
    mkdir -p "$OUTPUT_DIR" &&  \
    chown -R 100:1000 "$OUTPUT_DIR" &&  \
    chmod -R ug+rw "$OUTPUT_DIR"

USER consul-template

COPY ./$TEMPLATE_FILE $TEMPLATE_DIR

CMD /bin/consul-template \
    -consul-addr=${CONSUL_ADDR} \
    -template="${TEMPLATE_DIR}${TEMPLATE_FILE}:${OUTPUT_FILE}:${RELOAD_CMD}"
