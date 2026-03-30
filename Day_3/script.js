// const form = document.getElementById('form');
// const username = document.getElementById('username');
// const phno = document.getElementById('phno')
// const email = document.getElementById('email');
// const age = document.getElementById('age')
// const gender = document.getElementById('gender');
// const password = document.getElementById('password');
// const password2 = document.getElementById('password2');

// form.addEventListener('submit', e => {
// 	e.preventDefault();
	
// 	checkInputs();
// });

// function checkInputs() {
// 	const usernameValue = username.value.trim();
// 	const emailValue = email.value.trim();
// 	const passwordValue = password.value.trim();
// 	const password2Value = password2.value.trim();
//     const phnoValue = phno.value.trim();
//     const ageValue = age.value.trim();
//     const genderValue = getGenderValue();

// 	if(usernameValue === '') {
// 		setErrorFor(username, 'Username cannot be blank');
//         return;
// 	} else {
// 		setSuccessFor(username);
// 	}
	
// 	if(emailValue === '') {
// 		setErrorFor(email, 'Email cannot be blank');
//         return;
// 	} else if (!isEmail(emailValue)) {
// 		setErrorFor(email, 'Not a valid email');
//         return;
// 	} else {
// 		setSuccessFor(email);
// 	}
//     if(phnoValue === '') {
// 		setErrorFor(phno, 'phone no cannot be blank');
//         return;
// 	} else if (!isPhone(phnoValue)) {
// 		setErrorFor(phno, 'Not a valid phone no');
//         return;
// 	} else {
// 		setSuccessFor(phno);
// 	}
//     if (!genderValue) {
//         setErrorFor(gender, 'Please select a gender');
//         return;
//     } else {
//         setSuccessFor(gender);
//     }
//     if(ageValue === '') {
// 		setErrorFor(age, 'Age cannot be blank');
//         return;
// 	} else if (!isAge(ageValue)) {
// 		setErrorFor(age, 'Not a valid Age');
//         return;
// 	} else {
// 		setSuccessFor(age);
// 	}
    
	
// 	if(passwordValue === '') {
// 		setErrorFor(password, 'Password cannot be blank');
//         return;
// 	}
//     else if(!isPassword(passwordValue)){
//         setErrorFor(password, 'password length must be greater than 8')
//         return;
//     } else {
// 		setSuccessFor(password);
// 	}
	
// 	if(password2Value === '') {
// 		setErrorFor(password2, 'Confirm Password cannot be blank');
//         return
//     } else if(passwordValue !== password2Value) {
// 		setErrorFor(password2, 'Passwords does not match');
//         return;
// 	} else{
// 		setSuccessFor(password2);
// 	}
// }

// function setErrorFor(input, message) {
// 	const formControl = input.parentElement;
// 	const small = formControl.querySelector('small');
// 	formControl.className = 'form-control error';
// 	small.innerText = message;
// }

// function setSuccessFor(input) {
// 	const formControl = input.parentElement;
// 	formControl.className = 'form-control success';
// }
// function isPhone(phone) {
//     if (phone.length !== 10) return false;

//     if (phone[0] < '6' || phone[0] > '9') return false;

//     for (let i = 0; i < phone.length; i++) {
//         if (phone[i] < '0' || phone[i] > '9') return false;
//     }

//     return true;
// }

// function isEmail(email) {
//     if (!email.includes('@')) return false;

//     let parts = email.split('@');
//     if (parts.length !== 2) return false;
//     let username = parts[0];
//     let domain = parts[1];
//     if (username === '' || domain === '') return false;

//     if (!domain.includes('.')) return false;

//     if (domain.startsWith('.') || domain.endsWith('.')) return false;

//     return true;
// }
// function isAge(age) {
//     age = Number(age);

//     if (isNaN(age)) return false;

//     if (age <= 0 || age > 120) return false;

//     return true;
// }

// function getGenderValue() {
//     const genders = document.getElementsByName('gender');

//     for (let i = 0; i < genders.length; i++) {
//         if (genders[i].checked) {
//             return genders[i].value;
//         }
//     }
//     return null;
// }
// function isPassword(password){
//     return password.length >= 8;
// }

// // function isEmail(email) {
// // 	return /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email);
// // }

// // function isPhone(phone) {
// //     return /^(\+91[\-\s]?)?[6-9]\d{9}$/.test(phone);
// // }

const form = document.getElementById('form');
const username = document.getElementById('username');
const phno = document.getElementById('phno');
const email = document.getElementById('email');
const age = document.getElementById('age');
const gender = document.getElementById('gender');
const password = document.getElementById('password');
const password2 = document.getElementById('password2');

username.addEventListener('input', () => {
    if (username.value.trim() !== '') {
        setSuccessFor(username);
    }
});

email.addEventListener('input', () => {
    const value = email.value.trim();
    if (value === '') {
        setErrorFor(email, 'Email cannot be blank');
    } else if (!isEmail(value)) {
        setErrorFor(email, 'Not a valid email');
    } else {
        setSuccessFor(email);
    }
});

phno.addEventListener('input', () => {
    const value = phno.value.trim();
    if (value === '') {
        setErrorFor(phno, 'Phone cannot be blank');
    } else if (!isPhone(value)) {
        setErrorFor(phno, 'Not a valid phone no');
    } else {
        setSuccessFor(phno);
    }
});

age.addEventListener('input', () => {
    const value = age.value.trim();
    if (value === '') {
        setErrorFor(age, 'Age cannot be blank');
    } else if (!isAge(value)) {
        setErrorFor(age, 'Not a valid Age');
    } else {
        setSuccessFor(age);
    }
});

password.addEventListener('input', () => {
    const value = password.value.trim();
    if (value === '') {
        setErrorFor(password, 'Password cannot be blank');
    } else if (!isPassword(value)) {
        setErrorFor(password, 'Minimum 8 characters');
    } else {
        setSuccessFor(password);
    }
});

password2.addEventListener('input', () => {
    const value = password2.value.trim();
    if (value === '') {
        setErrorFor(password2, 'Confirm Password cannot be blank');
    } else if (value !== password.value.trim()) {
        setErrorFor(password2, 'Passwords do not match');
    } else {
        setSuccessFor(password2);
    }
});

const genders = document.getElementsByName('gender');
genders.forEach(g => {
    g.addEventListener('change', () => {
        const value = getGenderValue();
        if (!value) {
            setErrorFor(gender, 'Please select a gender');
        } else {
            setSuccessFor(gender);
        }
    });
});


form.addEventListener('submit', e => {
    e.preventDefault();
    checkInputs();
});

function checkInputs() {
    const usernameValue = username.value.trim();
    const emailValue = email.value.trim();
    const passwordValue = password.value.trim();
    const password2Value = password2.value.trim();
    const phnoValue = phno.value.trim();
    const ageValue = age.value.trim();
    const genderValue = getGenderValue();

    if (usernameValue === '') {
        setErrorFor(username, 'Username cannot be blank');
        return;
    } else {
        setSuccessFor(username);
    }

    if (emailValue === '') {
        setErrorFor(email, 'Email cannot be blank');
        return;
    } else if (!isEmail(emailValue)) {
        setErrorFor(email, 'Not a valid email');
        return;
    } else {
        setSuccessFor(email);
    }

    if (phnoValue === '') {
        setErrorFor(phno, 'Phone cannot be blank');
        return;
    } else if (!isPhone(phnoValue)) {
        setErrorFor(phno, 'Not a valid phone no');
        return;
    } else {
        setSuccessFor(phno);
    }

    if (!genderValue) {
        setErrorFor(gender, 'Please select a gender');
        return;
    } else {
        setSuccessFor(gender);
    }

    if (ageValue === '') {
        setErrorFor(age, 'Age cannot be blank');
        return;
    } else if (!isAge(ageValue)) {
        setErrorFor(age, 'Not a valid Age');
        return;
    } else {
        setSuccessFor(age);
    }

    if (passwordValue === '') {
        setErrorFor(password, 'Password cannot be blank');
        return;
    } else if (!isPassword(passwordValue)) {
        setErrorFor(password, 'Minimum 8 characters');
        return;
    } else {
        setSuccessFor(password);
    }

    if (password2Value === '') {
        setErrorFor(password2, 'Confirm Password cannot be blank');
        return;
    } else if (passwordValue !== password2Value) {
        setErrorFor(password2, 'Passwords does not match');
        return;
    } else {
        setSuccessFor(password2);
    }
    alert("Form submitted successfully!");
}


function setErrorFor(input, message) {
    const formControl = input.parentElement;
    const small = formControl.querySelector('small');
    formControl.className = 'form-control error';
    small.innerText = message;
}

function setSuccessFor(input) {
    const formControl = input.parentElement;
    formControl.className = 'form-control success';
}

function isPhone(phone) {
    if (phone.length !== 10) return false;
    if (phone[0] < '6' || phone[0] > '9') return false;

    for (let i = 0; i < phone.length; i++) {
        if (phone[i] < '0' || phone[i] > '9') return false;
    }
    return true;
}

function isEmail(email) {
    if (!email.includes('@')) return false;

    let parts = email.split('@');
    if (parts.length !== 2) return false;

    let username = parts[0];
    let domain = parts[1];

    if (username === '' || domain === '') return false;
    if (!domain.includes('.')) return false;
    if (domain.startsWith('.') || domain.endsWith('.')) return false;

    return true;
}

function isAge(age) {
    age = Number(age);
    if (isNaN(age)) return false;
    if (age <= 0 || age > 120) return false;
    return true;
}

function getGenderValue() {
    const genders = document.getElementsByName('gender');

    for (let i = 0; i < genders.length; i++) {
        if (genders[i].checked) {
            return genders[i].value;
        }
    }
    return null;
}

function isPassword(password) {
    return password.length >= 8;
}