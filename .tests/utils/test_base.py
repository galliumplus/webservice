from unittest import TestCase
from abc import ABC
import requests

from utils.expectations import Expectations


class TestBase(TestCase, ABC):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.base_url = "https://localhost:5443/api/"
        self.requests_options = {"verify": False}

    def prepend_base_url(self, url):
        if url.startswith("http://") or url.startswith("https://"):
            return url
        else:
            return self.base_url + url.lstrip("/")

    def set_authorization(self, auth):
        self.requests_options["auth"] = auth

    def setUp(self):
        requests.packages.urllib3.disable_warnings(
            requests.packages.urllib3.exceptions.InsecureRequestWarning
        )

    def unset_authorization(self):
        del self.requests_options["auth"]

    def head(self, url):
        return requests.head(self.prepend_base_url(url), **self.requests_options)

    def get(self, url, **params):
        return requests.get(self.prepend_base_url(url), **self.requests_options)

    def post(self, url, json):
        return requests.post(
            self.prepend_base_url(url), json=json, **self.requests_options
        )

    def put(self, url, json):
        return requests.put(
            self.prepend_base_url(url), json=json, **self.requests_options
        )

    def patch(self, url, json):
        return requests.patch(
            self.prepend_base_url(url), json=json, **self.requests_options
        )

    def delete(self, url):
        return requests.delete(self.prepend_base_url(url), **self.requests_options)

    def expect(self, value):
        return Expectations(self, value)
