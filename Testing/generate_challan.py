import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import time

class GenerateChallanTest(unittest.TestCase):
    def setUp(self):
        with open("url.txt", "r") as file:
            self.base_url = file.read().strip()
        self.driver = webdriver.Chrome()  
        self.driver.get(self.base_url + "/Home/Login")

        # Perform login
        self.driver.find_element(By.XPATH, "//input[@id='numlId']").send_keys("NUML-S24-10154")
        self.driver.find_element(By.XPATH, "//input[@id='password']").send_keys("aA2@34567")
        self.driver.find_element(By.XPATH, "//input[@type='submit']").click()
        time.sleep(5)

        # Navigate to Generate Challan page
        self.driver.get(self.base_url + "/GenerateChallan/generateChallan")

    def test_generate_challan_invalid(self):
        # Select Challan Type
        challan_type_dropdown = self.driver.find_element(By.ID, "challanDropDown")
        challan_type_dropdown.send_keys("Tuition Fee")

        # Select Session
        session_dropdown = self.driver.find_element(By.NAME, "sessionDropdown")
        session_dropdown.send_keys("Spring 2024")

        # Select Installment
        installment_dropdown = self.driver.find_element(By.NAME, "installmentsDropDown")
        installment_dropdown.send_keys("No Installments")

        # Submit the form
        submit_button = self.driver.find_element(By.XPATH, "//input[@value='Generate Challan']")
        submit_button.click()

        # Add appropriate wait and assertions as needed
        time.sleep(5)
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Challan Already Exists for Similar Challan Type and Session.")

    def test_generate_challan_valid(self):
        # Select Challan Type
        challan_type_dropdown = self.driver.find_element(By.ID, "challanDropDown")
        challan_type_dropdown.send_keys("Tuition Fee")

        # Select Session
        session_dropdown = self.driver.find_element(By.NAME, "sessionDropdown")
        session_dropdown.send_keys("Spring 2024")

        # Select Installment
        installment_dropdown = self.driver.find_element(By.NAME, "installmentsDropDown")
        installment_dropdown.send_keys("No Installments")

        # Submit the form
        submit_button = self.driver.find_element(By.XPATH, "//input[@value='Generate Challan']")
        submit_button.click()

        # Add appropriate wait and assertions as needed
        time.sleep(5)
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Challan Generated Successfully!")

    def tearDown(self):
        self.driver.quit()

if __name__ == "__main__":
    suite = unittest.TestLoader().loadTestsFromTestCase(GenerateChallanTest)
    tests = [suite._tests.pop(1), suite._tests.pop(0)]  # Swap test order
    suite.addTests(tests)
    unittest.TextTestRunner().run(suite)
