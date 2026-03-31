const inputs = document.querySelectorAll('.num');
const result = document.querySelector('.result');

inputs.forEach(input => {
    input.addEventListener('input', () => {
        
        let numbers = [];

        inputs.forEach(inp => {
            let value = inp.value.trim();

            if (value !== '' && !isNaN(value)) {
                numbers.push(Number(value));
            }
        });

        if (numbers.length === 0) {
            result.innerText = "START GIVING NUMBERS";
            return;
        }
        
        let max = numbers[0];

        for (let i = 1; i < numbers.length; i++) {
            if (numbers[i] > max) {
                max = numbers[i];
            }
        }

        result.innerText = "Greatest Number: " + max;
    });
});