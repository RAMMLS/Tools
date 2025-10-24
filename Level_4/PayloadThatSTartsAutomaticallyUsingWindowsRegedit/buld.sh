#!/bin/bash

build_cpp() {
  echo "Building C++: $1"
  x86_64-w64-mingw32-g++ -static -o "${1%.*}.exe" "$1"
}

build_go() {
  echo "Building Go: $1"
  GOOS=windows GOARCH=amd64 go build -o "${1%.*}.exe" "$1"
}

# Если переданы аргументы
if [ $# -eq 0 ]; then
  echo "Usage: build.sh <file>"
  echo "Supported: .cpp, .go"
  exit 1
fi

case "${1##*.}" in
cpp) build_cpp "$1" ;;
go) build_go "$1" ;;
*) echo "Unsupported file type: ${1##*.}" ;;
esac
