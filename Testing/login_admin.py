import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException
import time

class NumlPayLoginTest(unittest.TestCase):
    def setUp(self):
        with open("url.txt", "r") as file:
            self.base_url = file.read().strip()
        self.driver = webdriver.Chrome()  
        self.driver.get(self.base_url + "/Home/loginAdmin")

    def test_login_with_valid_credentials(self):
        self.driver.find_element(By.XPATH, "//input[@id='email_id']").send_keys("MohsinAbbas@numl.edu.pk")
        self.driver.find_element(By.XPATH, "//input[@id='password']").send_keys("aA2@345678")
        self.driver.find_element(By.XPATH, "//input[@type='submit']").click()

        time.sleep(5)  

        self.assertIn("/dashboard/deptdashboard", self.driver.current_url.lower())

    def test_login_with_invalid_credentials(self):
        username = "MohsinAbbas@numl.edu.pk"
        password = "11223344Tt^"
        
        self.driver.find_element(By.XPATH, "//input[@id='email_id']").send_keys(username)
        self.driver.find_element(By.XPATH, "//input[@id='password']").send_keys(password)
        self.driver.find_element(By.XPATH, "//input[@type='submit']").click()
        
        time.sleep(2)  
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "User Not Found!!")

    def tearDown(self):
        self.driver.quit()


if __name__ == "__main__":
    unittest.main()
