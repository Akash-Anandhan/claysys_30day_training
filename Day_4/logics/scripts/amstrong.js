const input = document.getElementById('display');
const result = document.querySelector('.result');

input.addEventListener('input', function () {

    let num = input.value;

    num = num.replace(/[^0-9]/g, '');
    input.value = num;

    if (num === '') {
        result.textContent = "ARMSTRONG";
        return;
    }

    let sum = 0;
    let digits = num.length;

    for (let i = 0; i < num.length; i++) {
        let digit = num[i];
        sum += Math.pow(digit, digits);
    }


    if (sum == num) {
        result.textContent = "ARMSTRONG NUMBER ✅";
    } else {
        result.textContent = "NOT AN ARMSTRONG ❌";
    }
});
