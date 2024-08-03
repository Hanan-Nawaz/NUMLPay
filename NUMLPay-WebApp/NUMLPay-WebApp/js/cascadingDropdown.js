$(function () {
    $('#campusDropdown').change(function () {
        $.getJSON('/Home/getFacultiesByCampusAsync?campusId=' + ($('#campusDropdown').val()), function (data) {
            $('#facultyDdl').empty();
            var items = '<option>Select Faculty</option>';
            $.each(data, function (i, faculty) {
                items += "<option value='" + faculty.Value + "'>" + faculty.Text + "</option>";
            });
            $('#facultyDdl').html(items);
        });
    });
    $('#facultyDdl').change(function () {
        $.getJSON('/Home/getDeptsByFacultyAsync?facultyId=' + ($('#facultyDdl').val()), function (data) {
            var items = '<option>Select Department</option>';
            $.each(data, function (i, dept) {
                items += "<option value='" + dept.Value + "'>" + dept.Text + "</option>";
            });
            $('#deptDdl').html(items);
        });
    });
});

$(function () {
    // On form submission
    $("form").submit(function () {
        // Get the selected value from the dropdown
        var selectedValueFaculty = $("#facultyDdl").val();
        var selectedValueDept = $("#deptDdl").val();
        var selectedValueDegree = $("#degreeDdl").val();

        // Update the hidden input field value
        $("input[name='faculty_id']").val(selectedValueFaculty);
        $("input[name='dept_id']").val(selectedValueDept);
        $("input[name='degree_id']").val(selectedValueDegree);
        $("input[name='shift_id']").val(selectedValueDegree);
    });
});