
// Assignments
function toggleAssignmentDetails(ele) {
    let openEle = ele.parentElement.querySelector('.assignmentListRow.show');
    if (openEle) { openEle.classList.remove('show'); }
    if (ele != openEle) { ele.classList.add('show'); }
}