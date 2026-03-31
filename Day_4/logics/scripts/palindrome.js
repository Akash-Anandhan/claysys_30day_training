
const input = document.getElementById('display');
const result = document.querySelector('.result');

input.addEventListener('input', function () {

    let num = input.value;

    num = num.replace(/[^0-9]/g, '');
    input.value = num;

    let isPalindrome = true;

    for (let i = 0; i < num.length / 2; i++) {
        if (num[i] !== num[num.length - 1 - i]) {
            isPalindrome = false;
            break;
        }
    }

    if (num === '') {
        result.textContent = "PALINDROME";
    } else if (isPalindrome) {
        result.textContent = "YES, IT'S A PALINDROME ✅";
    } else {
        result.textContent = "NOT A PALINDROME ❌";
    }
});






