import requests

if __name__ == "__main__":
    votesCookieValue = ""
    while True:
        headers = {"Cookie": votesCookieValue}
        resp = requests.get("http://kslweb1.spb.ctf.su/second/level22/", headers=headers)
        content = resp.content.decode("ascii")
        print(content)
        votesStart = content.find("votes=")
        if votesStart == -1:
            break
        votesEnd = content.find("\n", votesStart)
        votesCookieValue = content[votesStart:votesEnd]

