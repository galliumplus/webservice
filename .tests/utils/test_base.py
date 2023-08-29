from unittest import TestCase
from abc import ABC
import requests

from utils.expectations import Expectations


class TestBase(TestCase, ABC):
    __request_count = 0
    __total_time = 0

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.base_url = "https://localhost:5443/api/"
        self.requests_options = {"verify": False}
        self.count_requests = True

    @classmethod
    def request_count(cls):
        return cls.__request_count

    @classmethod
    def average_latency(cls):
        return cls.__total_time / cls.__request_count

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

    def stop_counting_requests(self):
        self.counting_requests = False

    def resume_counting_requests(self):
        self.counting_requests = True

    def __send(self, requests_method, *args, **kwargs):
        r = requests_method(*args, **kwargs)
        if self.count_requests:
            TestBase.__request_count += 1
            TestBase.__total_time += r.elapsed.total_seconds()
        return r

    def head(self, url):
        return self.__send(
            requests.head, self.prepend_base_url(url), **self.requests_options
        )

    def get(self, url, **params):
        return self.__send(
            requests.get, self.prepend_base_url(url), **self.requests_options
        )

    def post(self, url, json):
        return self.__send(
            requests.post,
            self.prepend_base_url(url),
            json=json,
            **self.requests_options
        )

    def put(self, url, json):
        return self.__send(
            requests.put, self.prepend_base_url(url), json=json, **self.requests_options
        )

    def patch(self, url, json):
        return self.__send(
            requests.patch,
            self.prepend_base_url(url),
            json=json,
            **self.requests_options
        )

    def delete(self, url):
        return self.__send(
            requests.delete, self.prepend_base_url(url), **self.requests_options
        )

    def expect(self, value):
        return Expectations(self, value)
