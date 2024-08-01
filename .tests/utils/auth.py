from requests.auth import AuthBase
import base64


class BasicAuth(AuthBase):
    def __init__(self, username, password, encoding="utf-8"):
        self.__username = username
        self.__password = password
        self.__encoding = encoding

    def __call__(self, request):
        payload = f"{self.__username}:{self.__password}".encode(self.__encoding)
        base64_payload = base64.b64encode(payload).decode("ascii")
        # modify and return the request
        request.headers["Authorization"] = f"Basic {base64_payload}"
        return request


class BearerAuth(AuthBase):
    def __init__(self, token):
        self.__token = token

    def __call__(self, request):
        # modify and return the request
        request.headers["Authorization"] = f"Bearer {self.__token}"
        return request


class SecretKeyAuth(AuthBase):
    def __init__(self, key):
        self.__key = key

    def __call__(self, request):
        # modify and return the request
        request.headers["Authorization"] = f"Secret {self.__key}"
        return request
