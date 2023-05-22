from requests.auth import AuthBase
import base64


class BasicAuth(AuthBase):
    def __init__(self, username, password, encoding="utf-8"):
        self.username = username
        self.password = password
        self.encoding = encoding

    def __call__(self, request):
        payload = f"{self.username}:{self.password}".encode(self.encoding)
        base64_payload = base64.b64encode(payload)
        # modify and return the request
        request.headers["Authorization"] = f"Basic {base64_payload}"
        return request


class BearerAuth(AuthBase):
    def __init__(self, token):
        self.token = token

    def __call__(self, request):
        # modify and return the request
        request.headers["Authorization"] = f"Bearer {self.token}"
        return request
