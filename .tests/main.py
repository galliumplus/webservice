import unittest
import urllib3
import requests
import decimal

from utils.launcher import Launcher

from tests.category_tests import CategoryTests
from tests.product_tests import ProductTests
from tests.role_tests import RoleTests
from tests.order_tests import OrderTests
from tests.user_tests import UserTests
from tests.access_tests import AccessTests
from tests.client_tests import ClientTests


if __name__ == "__main__":
    decimal.DefaultContext.prec = 2
    Launcher.launch("1.1.0")
