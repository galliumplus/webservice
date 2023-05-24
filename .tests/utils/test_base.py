from unittest import TestCase
from abc import ABC
import requests

from utils.expectations import Expectations


class TestBase(TestCase, ABC):
    __request_count = 0

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.base_url = "https://localhost:5443/api/"
        self.requests_options = {"verify": False}

    @classmethod
    def request_count(cls):
        return cls.__request_count

    def setUp(self):
        requests.packages.urllib3.disable_warnings(
            requests.packages.urllib3.exceptions.InsecureRequestWarning
        )

    def prepend_base_url(self, url):
        if url.startswith("http://") or url.startswith("https://"):
            return url
        else:
            return self.base_url + url.lstrip("/")

    def set_authentification(self, auth):
        self.requests_options["auth"] = auth

    def unset_authentification(self):
        if "auth" in self.requests_options:
            del self.requests_options["auth"]

    def head(self, url):
        TestBase.__request_count += 1
        return requests.head(self.prepend_base_url(url), **self.requests_options)

    def get(self, url, **params):
        TestBase.__request_count += 1
        return requests.get(self.prepend_base_url(url), **self.requests_options)

    def post(self, url, json):
        TestBase.__request_count += 1
        return requests.post(
            self.prepend_base_url(url), json=json, **self.requests_options
        )

    def put(self, url, json):
        TestBase.__request_count += 1
        return requests.put(
            self.prepend_base_url(url), json=json, **self.requests_options
        )

    def patch(self, url, json):
        TestBase.__request_count += 1
        return requests.patch(
            self.prepend_base_url(url), json=json, **self.requests_options
        )

    def delete(self, url):
        TestBase.__request_count += 1
        return requests.delete(self.prepend_base_url(url), **self.requests_options)

    def expect(self, value):
        TestBase.__request_count += 1
        return Expectations(self, value)
