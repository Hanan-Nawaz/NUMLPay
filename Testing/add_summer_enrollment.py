import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
import time

class AddSummerEnrollmentTest(unittest.TestCase):
    def setUp(self):
        # Load the WebDriver and open the URL
        with open("url.txt", "r") as file:
            self.base_url = file.read().strip()
        self.driver = webdriver.Chrome()  
        self.driver.get(self.base_url + "/Home/loginAdmin")

        # Perform login
        self.driver.find_element(By.XPATH, "//input[@id='email_id']").send_keys("MohsinAbbas@numl.edu.pk")
        self.driver.find_element(By.XPATH, "//input[@id='password']").send_keys("aA2@345678")
        self.driver.find_element(By.XPATH, "//input[@type='submit']").click()
        time.sleep(5)

        # Navigate to Add Summer Enrollment page
        self.driver.get(self.base_url + "/SummerEnrollment/addSummerEnrollment")

    def test_add_summer_enrollment_invalid(self):
        # Invalid test case 1: Adding a summer enrollment with duplicate NUMLID
        # Select Subject
        subject_dropdown = self.driver.find_element(By.ID, "subDdl")
        subject_dropdown.send_keys("AI")

        # Enter NUMLID
        numlid_input = self.driver.find_element(By.ID, "std_numl_id")
        numlid_input.send_keys("NUML-F20-10154")  # Same NUMLID as in the valid test case

        # Click Add Summer Enrollment button
        add_enrollment_button = self.driver.find_element(By.XPATH, "//input[@value='Add Summer Enrollment']")
        add_enrollment_button.click()

        # Add assertions based on the expected behavior after submitting the form
        # Wait for error message
        time.sleep(5)
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Enrollment Already Exists for the specified Subject and User.")

    def test_add_summer_enrollment_valid(self):
        # Valid test case 1: Adding a summer enrollment with valid data
        # Select Subject
        subject_dropdown = self.driver.find_element(By.ID, "subDdl")
        subject_dropdown.send_keys("AI")

        # Enter NUMLID
        numlid_input = self.driver.find_element(By.ID, "std_numl_id")
        numlid_input.send_keys("NUML-F20-10154")

        # Click Add Summer Enrollment button
        add_enrollment_button = self.driver.find_element(By.XPATH, "//input[@value='Add Summer Enrollment']")
        add_enrollment_button.click()

        # Add assertions based on the expected behavior after submitting the form
        # Wait for success message
        time.sleep(5)
        success_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = success_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "Enrollment Added Successfully!")

    def tearDown(self):
        # Quit the WebDriver
        self.driver.quit()

if __name__ == "__main__":
    suite = unittest.TestLoader().loadTestsFromTestCase(AddSummerEnrollmentTest)
    tests = [suite._tests.pop(1), suite._tests.pop(0)]  # Reversing the order to switch between valid and invalid
    suite.addTests(tests)
    unittest.TextTestRunner().run(suite)
