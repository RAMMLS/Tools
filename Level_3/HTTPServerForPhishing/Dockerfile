FROM alpine:latest 
LABEL MAINTAINER = "https://github.com/RAMMLS/Tools/Level_3/HTTPServerForPhishing"
WORKDIR /Tools/Level_3/HTTPServerForPhishing/
ADD . /HTTPServerForPhishing
RUN apk add --no-cache bash ncurses cutl unzip wget php 
CMD "./main.sh"
