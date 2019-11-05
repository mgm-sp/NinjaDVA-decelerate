#!/bin/bash
set -e

internalNetwork="172.23.42.1/26"


# create temporary ssh config file
sshConfig=$(mktemp)
function finish {
  rm -f $sshConfig
}
trap finish EXIT


cd gateway_vm
vagrant ssh-config > $sshConfig
sshuttle -e "ssh -F $sshConfig" -r gateway $internalNetwork
