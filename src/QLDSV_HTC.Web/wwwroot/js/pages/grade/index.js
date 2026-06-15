document.addEventListener('DOMContentLoaded', function () {
    let currentMaLtc = 0;
    const tbody = document.getElementById('gradesTbody');
    const statusMsg = document.getElementById('statusMessage');
    const btnLoadStudents = document.getElementById('btnLoadStudents');
    const btnSaveGrades = document.getElementById('btnSaveGrades');

    function getGpaInfo(tb) {
        if (tb === null || isNaN(tb)) return { gpa: '-', letter: '-' };
        if (tb >= 9.0) return { gpa: 4.0, letter: 'A+' };
        if (tb >= 8.5) return { gpa: 3.7, letter: 'A' };
        if (tb >= 8.0) return { gpa: 3.5, letter: 'B+' };
        if (tb >= 7.0) return { gpa: 3.0, letter: 'B' };
        if (tb >= 6.5) return { gpa: 2.5, letter: 'C+' };
        if (tb >= 5.5) return { gpa: 2.0, letter: 'C' };
        if (tb >= 5.0) return { gpa: 1.5, letter: 'D+' };
        if (tb >= 4.0) return { gpa: 1.0, letter: 'D' };
        return { gpa: 0.0, letter: 'F' };
    }

    // Load data
    btnLoadStudents.addEventListener('click', async function () {
        const year = document.getElementById('filterYear').value;
        const semester = document.getElementById('filterSemester').value;
        const subjectId = document.getElementById('filterSubject').value;
        const group = document.getElementById('filterGroup').value;
        const studentIdFilter = document.getElementById('filterStudentId').value;
        const studentNameFilter = document.getElementById('filterStudentName').value;

        statusMsg.innerHTML = '<i class="bi bi-hourglass-split me-1"></i> Đang tải dữ liệu...';
        tbody.innerHTML = '<tr><td colspan="8" class="text-center py-4"><div class="spinner-border text-primary" role="status"></div></td></tr>';

        try {
            const queryParams = new URLSearchParams({
                year: year || '',
                semester: semester || '0',
                subjectId: subjectId || '',
                group: group || '0',
                studentId: studentIdFilter || '',
                studentName: studentNameFilter || ''
            });

            const response = await fetch('/api/grades/students?' + queryParams.toString(), {
                method: 'GET',
                headers: {
                    'Accept': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Lỗi khi tải dữ liệu.');
            }

            const data = await response.json();
            tbody.innerHTML = '';

            if (data && data.length > 0) {
                currentMaLtc = data[0].maLtc;
                
                data.forEach((student, index) => {
                    const tr = document.createElement('tr');
                    tr.className = 'hover-row transition-base';
                    tr.dataset.studentid = student.studentId;
                    tr.dataset.maltc = student.maLtc || 0;
                    
                    const attVal = student.attendanceGrade !== null ? student.attendanceGrade : '';
                    const midVal = student.midtermGrade !== null ? student.midtermGrade : '';
                    const finVal = student.finalGrade !== null ? student.finalGrade : '';
                    const totVal = student.totalGrade !== null ? student.totalGrade : '-';

                    const gpaInfo = student.totalGrade !== null ? getGpaInfo(student.totalGrade) : { gpa: '-', letter: '-' };

                    let badgeClass = 'bg-secondary';
                    let badgeText = 'Chưa có';
                    if (student.totalGrade !== null) {
                        if (student.totalGrade >= 4) {
                            badgeClass = 'bg-success-subtle text-success';
                            badgeText = `Đạt (${gpaInfo.letter})`;
                        } else {
                            badgeClass = 'bg-danger-subtle text-danger';
                            badgeText = `Rớt (${gpaInfo.letter})`;
                        }
                    }

                    tr.innerHTML = `
                        <td class="ps-4">
                            <span class="text-muted small fw-bold">${(index + 1).toString().padStart(2, '0')}</span>
                        </td>
                        <td>
                            <span class="fw-bold font-monospace text-primary bg-primary-subtle px-2 py-1 rounded small border border-primary-subtle">
                                ${student.studentId}
                            </span>
                        </td>
                        <td>
                            <div class="fw-semibold text-dark">${student.lastName} ${student.firstName}</div>
                        </td>
                        <td>
                            <div class="input-group input-group-sm justify-content-center">
                                <input type="number" class="form-control text-center shadow-none border-dashed grade-input cc" 
                                       value="${attVal}" min="0" max="10" step="0.5" style="max-width: 70px; border-radius: 6px;">
                            </div>
                        </td>
                        <td>
                            <div class="input-group input-group-sm justify-content-center">
                                <input type="number" class="form-control text-center shadow-none border-dashed grade-input gk" 
                                       value="${midVal}" min="0" max="10" step="0.5" style="max-width: 70px; border-radius: 6px;">
                            </div>
                        </td>
                        <td>
                            <div class="input-group input-group-sm justify-content-center">
                                <input type="number" class="form-control text-center shadow-none border-dashed grade-input ck" 
                                       value="${finVal}" min="0" max="10" step="0.5" style="max-width: 70px; border-radius: 6px;">
                            </div>
                        </td>
                        <td class="text-center">
                            <span class="fw-bold text-success fs-5 total-grade">${totVal}</span>
                        </td>
                        <td class="text-center pe-4">
                            <div class="d-flex flex-column align-items-center">
                                <span class="fw-bold text-primary fs-5 gpa-grade">${gpaInfo.gpa}</span>
                                <span class="badge ${badgeClass} small rounded-pill px-3 shadow-sm">
                                    ${badgeText}
                                </span>
                            </div>
                        </td>
                    `;
                    tbody.appendChild(tr);
                });
                
                statusMsg.innerHTML = `<i class="bi bi-check-circle text-success me-1"></i> Tải thành công ${data.length} sinh viên.`;
                bindEvents();
            } else {
                tbody.innerHTML = '<tr><td colspan="8" class="text-center py-4 text-muted">Không tìm thấy sinh viên nào cho lớp này.</td></tr>';
                statusMsg.innerHTML = '<i class="bi bi-info-circle me-1"></i> Không tìm thấy dữ liệu.';
            }
        } catch (error) {
            tbody.innerHTML = '<tr><td colspan="8" class="text-center py-4 text-danger">Đã xảy ra lỗi khi tải dữ liệu.</td></tr>';
            statusMsg.innerHTML = '<i class="bi bi-exclamation-circle text-danger me-1"></i> Lỗi hệ thống.';
            console.error(error);
        }
    });

    function bindEvents() {
        const inputs = document.querySelectorAll('.grade-input');
        inputs.forEach(input => {
            input.addEventListener('input', function() {
                // Auto-format leading zeros for integers (e.g. "01" -> "1", "00" -> "0")
                if (/^0\d+$/.test(this.value)) {
                    this.value = parseInt(this.value, 10).toString();
                }

                let val = parseFloat(this.value);
                if (!isNaN(val)) {
                    if (val < 0) {
                        this.value = 0;
                    } else if (val > 10) {
                        this.value = 10;
                    }
                }

                const tr = this.closest('tr');
                
                let cc = parseFloat(tr.querySelector('.cc').value);
                let gk = parseFloat(tr.querySelector('.gk').value);
                let ck = parseFloat(tr.querySelector('.ck').value);

                const totalEl = tr.querySelector('.total-grade');
                const gpaEl = tr.querySelector('.gpa-grade');
                const badgeEl = tr.querySelector('.badge');

                if (isNaN(cc) || isNaN(gk) || isNaN(ck)) {
                    totalEl.textContent = '-';
                    gpaEl.textContent = '-';
                    badgeEl.className = 'badge bg-secondary small rounded-pill px-3 shadow-sm';
                    badgeEl.textContent = 'Chưa có';
                    return;
                }

                let total = cc * 0.1 + gk * 0.3 + ck * 0.6;
                total = Math.round(total * 10) / 10; // Round to 1 decimal place

                totalEl.textContent = total;
                const gpaInfo = getGpaInfo(total);
                gpaEl.textContent = gpaInfo.gpa;
                
                if (total >= 4) {
                    badgeEl.className = 'badge bg-success-subtle text-success small rounded-pill px-3 shadow-sm';
                    badgeEl.textContent = `Đạt (${gpaInfo.letter})`;
                } else {
                    badgeEl.className = 'badge bg-danger-subtle text-danger small rounded-pill px-3 shadow-sm';
                    badgeEl.textContent = `Rớt (${gpaInfo.letter})`;
                }
            });

            // Sanitize decimals and trailing dots on blur/change (e.g. "01.5" -> "1.5", "5." -> "5")
            input.addEventListener('change', function() {
                let val = parseFloat(this.value);
                if (!isNaN(val)) {
                    this.value = val.toString();
                }
            });
        });
    }

    // Save grades
    btnSaveGrades.addEventListener('click', async function () {
        const rows = tbody.querySelectorAll('tr[data-studentid]');
        if (rows.length === 0) {
            window.showToast('Không có dữ liệu điểm để lưu.', 'info');
            return;
        }

        let grades = [];
        let hasInvalidGrade = false;

        rows.forEach(function (tr) {
            const studentId = tr.dataset.studentid;
            const maLtc = parseInt(tr.dataset.maltc);
            
            const ccStr = tr.querySelector('.cc').value;
            const gkStr = tr.querySelector('.gk').value;
            const ckStr = tr.querySelector('.ck').value;

            const ccVal = ccStr ? parseFloat(ccStr) : null;
            const gkVal = gkStr ? parseFloat(gkStr) : null;
            const ckVal = ckStr ? parseFloat(ckStr) : null;

            if ((ccVal !== null && (ccVal < 0 || ccVal > 10)) ||
                (gkVal !== null && (gkVal < 0 || gkVal > 10)) ||
                (ckVal !== null && (ckVal < 0 || ckVal > 10))) {
                hasInvalidGrade = true;
            }

            grades.push({
                maLtc: maLtc,
                studentId: studentId,
                attendanceGrade: ccVal,
                midtermGrade: gkVal,
                finalGrade: ckVal
            });
        });

        if (hasInvalidGrade) {
            window.showToast('Tất cả điểm nhập vào phải từ 0 đến 10.', 'danger');
            return;
        }

        const confirmed = await window.showConfirmDialog('Lưu bảng điểm?', 'Bạn có chắc chắn muốn lưu các điểm này không?', { confirmText: 'Đồng ý', cancelText: 'Hủy' });
        
        if (confirmed) {
            try {
                const response = await fetch('/api/grades/save', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: JSON.stringify(grades)
                });

                const res = await response.json();
                if (response.ok) {
                    window.showToast(res.message || 'Thành công', 'success');
                } else {
                    window.showToast(res.message || 'Không thể lưu bảng điểm', 'danger');
                }
            } catch (error) {
                window.showToast('Lỗi hệ thống khi lưu bảng điểm', 'danger');
                console.error(error);
            }
        }
    });
});
