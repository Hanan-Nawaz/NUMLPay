import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
import time

class AddOtherFeeTest(unittest.TestCase):
    def setUp(self):
        with open("url.txt", "r") as file:
            self.base_url = file.read().strip()
        self.driver = webdriver.Chrome()  
        self.driver.get(self.base_url + "/Home/loginAdmin")

        # Perform login
        self.driver.find_element(By.XPATH, "//input[@id='email_id']").send_keys("hamza@numl.edu.pk")
        self.driver.find_element(By.XPATH, "//input[@id='password']").send_keys("aA2@345678")
        self.driver.find_element(By.XPATH, "//input[@type='submit']").click()
        time.sleep(5)

        # Navigate to Generate Challan page
        self.driver.get(self.base_url + "/OtherFee/addOtherFee")

    def test_add_other_fee_in_valid(self):
        # Perform actions to add other fee
        # Select Admission Session
        session_dropdown = self.driver.find_element(By.ID, "session")
        session_dropdown.send_keys("Spring 2024")

        # Select Fee For
        fee_for_dropdown = self.driver.find_element(By.ID, "challanDropdown")
        fee_for_dropdown.send_keys("Hostel Fee")

        # Enter Total Fee
        total_fee_input = self.driver.find_element(By.ID, "total_fee")
        total_fee_input.send_keys("25000")

        # Enter Security Fee
        security_fee_input = self.driver.find_element(By.ID, "securityFee")
        security_fee_input.send_keys("5000")

        # Select Campus
        campus_dropdown = self.driver.find_element(By.ID, "campusDropdown")
        campus_dropdown.send_keys("Main Campus")

        # Click submit button
        submit_button = self.driver.find_element(By.XPATH, "//input[@value='Add Fee']")
        submit_button.click()

        # Add assertions based on the expected behavior after submitting the form
        # For example:
        # Wait for success message
        time.sleep(5)
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Fee Already Exists.")

    def test_add_other_fee_valid(self):
        # Perform actions to add other fee
        # Select Admission Session
        session_dropdown = self.driver.find_element(By.ID, "session")
        session_dropdown.send_keys("Spring 2024")

        # Select Fee For
        fee_for_dropdown = self.driver.find_element(By.ID, "challanDropdown")
        fee_for_dropdown.send_keys("Hostel Fee")

        # Enter Total Fee
        total_fee_input = self.driver.find_element(By.ID, "total_fee")
        total_fee_input.send_keys("30000")

        # Enter Security Fee
        security_fee_input = self.driver.find_element(By.ID, "securityFee")
        security_fee_input.send_keys("3000")

        # Select Campus
        campus_dropdown = self.driver.find_element(By.ID, "campusDropdown")
        campus_dropdown.send_keys("Main Campus")

        # Click submit button
        submit_button = self.driver.find_element(By.XPATH, "//input[@value='Add Fee']")
        submit_button.click()

        # Add assertions based on the expected behavior after submitting the form
        # Wait for success message
        time.sleep(5)
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Fee Security Added Successfully!")

    def tearDown(self):
        # Quit the WebDriver
        self.driver.quit()

if __name__ == "__main__":
    suite = unittest.TestLoader().loadTestsFromTestCase(AddOtherFeeTest)
    tests = [suite._tests.pop(1), suite._tests.pop(0)]  # Swap test order
    suite.addTests(tests)
    unittest.TextTestRunner().run(suite)