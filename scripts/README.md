Scripts
=======

## simulateAudience.py
This script logs a given number of users with random names into the application and each of these users votes
with random values at random intervals until the script is terminated.
Useful to test the application and create fancy screenshots.

Usage: `python simulateAudience.py rootUrl numberOfUsers`, e.g.
```
python simulateAudience.py http://localhost:59901/ 10
```
if the application runs at `http://localhost:59901/` and you want to have ten users.
Terminate with Ctrl+C.

## NinjaDVA\_tunnel.sh
Tunnels into the internal NinjaDVA network using sshuttle.
Execute when you're in the NinjaDVA main directory.

## NinjaDVA\_getIPs.sh
Lists the IP addresses for all VMs.
Execute when you're in the NinjaDVA main directory.
