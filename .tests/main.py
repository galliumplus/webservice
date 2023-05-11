import unittest
import urllib3
import requests

from utils.launcher import Launcher

from tests.user_tests import UserTests
from tests.role_tests import RoleTests


if __name__ == "__main__":
    Launcher.launch()
