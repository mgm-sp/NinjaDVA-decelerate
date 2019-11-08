#!/bin/bash
set -e

for f in **/Vagrantfile; do
  dir=$(dirname $f)
  if [[ "$dir" = "your_vulnerable_vm" ]] || [[ "$dir" = "gateway_vm" ]]; then
    continue
  fi
  ipAddr=$(
    cd $dir
    vagrant ssh -c "ip addr" | grep "inet " | grep eth1 | grep -v secondary | cut -d' ' -f6 | cut -d'/' -f1
  )
  echo $dir $ipAddr
done
