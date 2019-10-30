#!/usr/bin/python3
import sys, time, urllib.parse, threading, random, ctypes
import requests

if len(sys.argv) < 3:
    print('Usage: %s rootUrl numberOfUsers' % sys.argv[0])
    sys.exit(1)

rootUrl = sys.argv[1]
numberOfUsers = int(sys.argv[2])


class Session(requests.Session):
    def __init__(self, username):
        super().__init__()
        self.username = username

    def __enter__(self):
        super().__enter__()

        # Get CSRF token from login page:
        print('[%s] Getting CSRF token for login ...' % self.username)
        response = self.get(rootUrl)
        csrfToken = response.text.split('value="')[2].split('"')[0]
        print('[%s] Got CSRF token for login: %s' % (self.username, csrfToken))

        # Login:
        print('[%s] Logging in ...' % self.username)
        self.post(rootUrl, data={'Name': self.username, '__RequestVerificationToken': csrfToken})
        print('[%s] Logged in.' % self.username)

        # Return session:
        return self

    def __exit__(self, type, value, tb):
        # Logout:
        print('[%s] Logging out ...' % self.username)
        self.get(urllib.parse.urljoin(rootUrl, '/UserArea/Logout'))
        print('[%s] Logged out.' % self.username)

        super().__exit__(type, value, tb)

    def vote(self, value):
        # Get CSRF token from voting page:
        print('[%s] Getting CSRF token for voting ...' % self.username)
        response = self.get(urllib.parse.urljoin(rootUrl, '/UserArea'))
        csrfToken = response.text.split('value="')[2].split('"')[0]
        print('[%s] Got CSRF token for voting: %s' % (self.username, csrfToken))

        # Vote:
        print('[%s] Voting ...' % self.username)
        self.post(urllib.parse.urljoin(rootUrl, '/UserArea'),
            data={'SpeedChoice': value, '__RequestVerificationToken': csrfToken})
        print('[%s] Voted.' % self.username)


class BotThread(threading.Thread):
    exit = False

    def run(self):
        with Session(self.getName()) as session:
            delayLeft = random.uniform(0, 60)
            while not self.exit:
                if delayLeft <= 0:
                    delayLeft = random.uniform(5, 120)
                    session.vote(random.randint(-100, 100))
                else:
                    delayLeft -= 1
                    time.sleep(1)


threads = []
prefix = random.randrange(10000)
for i in range(numberOfUsers):
    thread = BotThread(name="bot%d-%d" % (prefix, i))
    thread.start()
    threads.append(thread)

try:
    while True:
        time.sleep(1)
finally:
    for thread in threads:
        thread.exit = True
