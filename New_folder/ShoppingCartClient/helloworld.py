# Python program to print the first 50 prime numbers using for loops

import itertools

def is_prime(n):
    if n < 2:
        return False
    for i in range(2, int(n ** 0.5) + 1):
        if n % i == 0:
            return False
    return True

# Find and print the first 50 prime numbers using a for loop
primes = []

for num in itertools.count(2):
    if is_prime(num):
        primes.append(num)
        if len(primes) == 50:
            break

# Display the results
print('First 50 prime numbers:')

for i, prime in enumerate(primes, 1):
    print(f'{prime:4}', end='')
    if i % 10 == 0:
        print()  # New line after every 10 primes

print(f'\n\nTotal primes printed: {len(primes)}')