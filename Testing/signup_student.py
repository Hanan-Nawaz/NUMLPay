import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException
import time
from selenium.webdriver.common.action_chains import ActionChains

class NumlPaySignupTest(unittest.TestCase):
    def setUp(self):
        with open("url.txt", "r") as file:
            self.base_url = file.read().strip()
        self.driver = webdriver.Chrome()  
        self.driver.get(self.base_url + "/Home/Register")

    def test_signup_with_invalid_credentials(self):
        # Fill out the registration form with valid data
        
        # Enter NUML ID
        numl_id_input = self.driver.find_element(By.XPATH, "//input[@id='numl_id']")
        numl_id_input.send_keys("NUML-S24-10155")

        # Enter Name
        name_input = self.driver.find_element(By.XPATH, "//input[@id='name']")
        name_input.send_keys("Jamaml Ali")

        # Enter Password
        password_input = self.driver.find_element(By.XPATH, "//input[@id='password']")
        password_input.send_keys("aA2@34567")

         # Enter Father's Name
        father_name_input = self.driver.find_element(By.XPATH, "//input[@id='father_name']")
        father_name_input.send_keys("Muhammad Ali")

        # Enter Date of Birth
        dob_input = self.driver.find_element(By.XPATH, "//input[@id='date_of_birth']")
        ActionChains(self.driver).move_to_element(dob_input).click().perform()
        dob_input.send_keys("12/24/2002")

        # Enter Email
        email_input = self.driver.find_element(By.XPATH, "//input[@id='email']")
        email_input.send_keys("ali@gmail.com")

        # Enter Contact Number
        contact_input = self.driver.find_element(By.XPATH, "//input[@id='contact']")
        contact_input.send_keys("1234567890")

        # Enter CNIC
        cnic_input = self.driver.find_element(By.XPATH, "//input[@id='nic']")
        cnic_input.send_keys("1234567890123")

        # Select Campus
        campus_dropdown = self.driver.find_element(By.XPATH, "//select[@id='campusDropdown']")
        campus_dropdown.send_keys("Main Campus")

        # Select Faculty
        faculty_dropdown = self.driver.find_element(By.XPATH, "//select[@id='facultyDdl']")
        faculty_dropdown.send_keys("Faculty of CS and Engineering")

        # Select Department
        dept_dropdown = self.driver.find_element(By.XPATH, "//select[@id='deptDdl']")
        dept_dropdown.send_keys("Dept. of SE")

        # Select Academic Level
        academic_dropdown = self.driver.find_element(By.XPATH, "//select[@id='academicDdl']")
        academic_dropdown.send_keys("BS")

        # Select Degree
        degree_dropdown = self.driver.find_element(By.XPATH, "//select[@id='degreeDdl']")
        degree_dropdown.send_keys("Software Engineering Morning")


        # Select Admission Session
        admission_session_dropdown = self.driver.find_element(By.XPATH, "//select[@id='admission_session']")
        admission_session_dropdown.send_keys("Spring 2024")

        # Select Fee Plan
        fee_plan_dropdown = self.driver.find_element(By.XPATH, "//select[@id='fee_plan']")
        fee_plan_dropdown.send_keys("Kinship - 4%")

        # Select Hostel Fees
        hostel_dropdown = self.driver.find_element(By.XPATH, "//select[@name='hostelDdl']")
        hostel_dropdown.send_keys("Yes")

        # Select Bus Fees
        bus_dropdown = self.driver.find_element(By.XPATH, "//select[@name='busDdl']")
        bus_dropdown.send_keys("Yes")

        # Upload Picture
        image_input = self.driver.find_element(By.XPATH, "//input[@id='imageInput']")
        image_input.send_keys("E:/Wallpaper/numl.jpeg")

        # Add appropriate wait  as needed
        time.sleep(10)

        # Submit the form
        submit_button = self.driver.find_element(By.XPATH, "//input[@class='btnLogin mb-4']")
        self.driver.execute_script("arguments[0].scrollIntoView();", submit_button)
        submit_button.click()

        # Add appropriate wait and assertions as needed
        time.sleep(5)
        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "User Already Exists with this NUML ID.")

    def test_signup_with_valid_credentials(self):
        # Fill out the registration form with valid data
        
        # Enter NUML ID
        numl_id_input = self.driver.find_element(By.XPATH, "//input[@id='numl_id']")
        numl_id_input.send_keys("NUML-S24-10155")

        # Enter Name
        name_input = self.driver.find_element(By.XPATH, "//input[@id='name']")
        name_input.send_keys("Jamaml Ali")

        # Enter Password
        password_input = self.driver.find_element(By.XPATH, "//input[@id='password']")
        password_input.send_keys("aA2@34567")

         # Enter Father's Name
        father_name_input = self.driver.find_element(By.XPATH, "//input[@id='father_name']")
        father_name_input.send_keys("Muhammad Ali")

        # Enter Date of Birth
        dob_input = self.driver.find_element(By.XPATH, "//input[@id='date_of_birth']")
        ActionChains(self.driver).move_to_element(dob_input).click().perform()
        dob_input.send_keys("12/24/2002")

        # Enter Email
        email_input = self.driver.find_element(By.XPATH, "//input[@id='email']")
        email_input.send_keys("ali@gmail.com")

        # Enter Contact Number
        contact_input = self.driver.find_element(By.XPATH, "//input[@id='contact']")
        contact_input.send_keys("1234567890")

        # Enter CNIC
        cnic_input = self.driver.find_element(By.XPATH, "//input[@id='nic']")
        cnic_input.send_keys("1234567890123")

        # Select Campus
        campus_dropdown = self.driver.find_element(By.XPATH, "//select[@id='campusDropdown']")
        campus_dropdown.send_keys("Main Campus")

        # Select Faculty
        faculty_dropdown = self.driver.find_element(By.XPATH, "//select[@id='facultyDdl']")
        faculty_dropdown.send_keys("Faculty of CS and Engineering")

        # Select Department
        dept_dropdown = self.driver.find_element(By.XPATH, "//select[@id='deptDdl']")
        dept_dropdown.send_keys("Dept. of SE")

        # Select Academic Level
        academic_dropdown = self.driver.find_element(By.XPATH, "//select[@id='academicDdl']")
        academic_dropdown.send_keys("BS")

        # Select Degree
        degree_dropdown = self.driver.find_element(By.XPATH, "//select[@id='degreeDdl']")
        degree_dropdown.send_keys("Software Engineering Morning")


        # Select Admission Session
        admission_session_dropdown = self.driver.find_element(By.XPATH, "//select[@id='admission_session']")
        admission_session_dropdown.send_keys("Spring 2024")

        # Select Fee Plan
        fee_plan_dropdown = self.driver.find_element(By.XPATH, "//select[@id='fee_plan']")
        fee_plan_dropdown.send_keys("Kinship - 4%")

        # Select Hostel Fees
        hostel_dropdown = self.driver.find_element(By.XPATH, "//select[@name='hostelDdl']")
        hostel_dropdown.send_keys("Yes")

        # Select Bus Fees
        bus_dropdown = self.driver.find_element(By.XPATH, "//select[@name='busDdl']")
        bus_dropdown.send_keys("Yes")

        # Upload Picture
        image_input = self.driver.find_element(By.XPATH, "//input[@id='imageInput']")
        image_input.send_keys("E:/Wallpaper/numl.jpeg")

        # Add appropriate wait  as needed
        time.sleep(10)

        # Submit the form
        submit_button = self.driver.find_element(By.XPATH, "//input[@class='btnLogin mb-4']")
        self.driver.execute_script("arguments[0].scrollIntoView();", submit_button)
        submit_button.click()

        # Add appropriate wait and assertions as needed
        time.sleep(5)

        error_message = self.driver.find_element(By.XPATH, "//div[@id='customAlert']").text.strip()
        first_line = error_message.split('\n')[0].strip('"')
        self.assertEqual(first_line, "User Added Successfully!")

    def tearDown(self):
        self.driver.quit()


if __name__ == "__main__":
    suite = unittest.TestLoader().loadTestsFromTestCase(NumlPaySignupTest)
    tests = [suite._tests.pop(1), suite._tests.pop(0)]  # Swap test order
    suite.addTests(tests)
    unittest.TextTestRunner().run(suite)
