import unittest
import urllib3
import requests

from utils.launcher import Launcher

from tests.category_tests import CategoryTests
from tests.product_tests import ProductTests
from tests.role_tests import RoleTests
from tests.order_tests import OrderTests
from tests.user_tests import UserTests


if __name__ == "__main__":
    Launcher.launch("0.2.1")
