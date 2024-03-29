#!/usr/bin/python3
import sys, time, urllib.parse, threading, random, ctypes, os.path
import requests

if len(sys.argv) < 4:
    print('Usage: %s rootUrl roomCode numberOfUsers' % sys.argv[0])
    sys.exit(1)

rootUrl = sys.argv[1]
roomCode = sys.argv[2]
numberOfUsers = int(sys.argv[3])


class Session(requests.Session):
    def __init__(self, username, roomCode):
        super().__init__()
        self.username = username
        self.roomCode = roomCode

    def __enter__(self):
        super().__enter__()

        # Get CSRF token from login page:
        print('[%s] Getting CSRF token for login ...' % self.username)
        response = self.get(rootUrl)
        csrfToken = response.text.split('type="hidden" value="')[1].split('"')[0]
        print('[%s] Got CSRF token for login: %s' % (self.username, csrfToken))

        # Login:
        print('[%s] Logging in ...' % self.username)
        self.post(rootUrl, data={
            'Name': self.username,
            'RoomCode': self.roomCode,
            '__RequestVerificationToken': csrfToken
        })
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

    def __init__(self, *args, **kwargs):
        self.roomCode = kwargs['roomCode']
        del kwargs['roomCode']
        super().__init__(*args, **kwargs)

    def run(self):
        with Session(self.getName(), self.roomCode) as session:
            delayLeft = random.uniform(0, 60)
            while not self.exit:
                if delayLeft <= 0:
                    delayLeft = random.uniform(5, 120)
                    session.vote(random.randint(-100, 100))
                else:
                    delayLeft -= 1
                    time.sleep(1)


def getName():
    # Waterman's Reservoir Algorithm
    # see https://stackoverflow.com/questions/3540288/how-do-i-read-a-random-line-from-one-file-in-python#tab-top
    with open(os.path.join(os.path.dirname(__file__), 'names.txt'), 'r') as f:
        line = next(f)
        for num, nline in enumerate(f, 2):
            if random.randrange(num): continue
            line = nline
    return line.strip()

threads = []
for i in range(numberOfUsers):
    thread = BotThread(name=getName(), roomCode=roomCode)
    thread.start()
    threads.append(thread)

try:
    while True:
        time.sleep(1)
finally:
    for thread in threads:
        thread.exit = True
