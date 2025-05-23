import requests

if __name__ == "__main__":
    locationHeader = "Location: "
    url = "https://redirect.board.course.ugractf.ru/021a8b0ebcb2558c/"
    while True:
        resp = requests.get(url)
        content = resp.content.decode("ascii")
        print(content)
        if not content.startswith(locationHeader):
            break
        urlPart = content[len(locationHeader):]
        url = f"https://{urlPart}"