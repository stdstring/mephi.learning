import cvxpy as cp

if __name__ == "__main__":
    x11 = cp.Variable(integer=True, nonneg=True)
    x12 = cp.Variable(integer=True, nonneg=True)
    x13 = cp.Variable(integer=True, nonneg=True)
    x14 = cp.Variable(integer=True, nonneg=True)
    x15 = cp.Variable(integer=True, nonneg=True)
    x21 = cp.Variable(integer=True, nonneg=True)
    x22 = cp.Variable(integer=True, nonneg=True)
    x23 = cp.Variable(integer=True, nonneg=True)
    x24 = cp.Variable(integer=True, nonneg=True)
    x25 = cp.Variable(integer=True, nonneg=True)
    x31 = cp.Variable(integer=True, nonneg=True)
    x32 = cp.Variable(integer=True, nonneg=True)
    x33 = cp.Variable(integer=True, nonneg=True)
    x34 = cp.Variable(integer=True, nonneg=True)
    x35 = cp.Variable(integer=True, nonneg=True)
    objective = cp.Minimize(4 * x11 + 3 * x12 + 6 * x13 + 7 * x14 + 9 * x15 +
                            5 * x21 + 4 * x22 + 8 * x23 + 6 * x24 +7 * x25 +
                            9 * x31 + 7 * x32 + 4 * x33 + 5 * x34 + 6 * x35)
    constraints = [
        x11 + x12 + x13 + x14 + x15 <= 100,
        x21 + x22 + x23 + x24 + x25 <= 150,
        x31 + x32 + x33 + x34 + x35 <= 200,
        x11 + x21 + x31 == 80,
        x12 + x22 + x32 == 90,
        x13 + x23 + x33 == 110,
        x14 + x24 + x34 == 120,
        x15 + x25 + x35 == 50
    ]
    problem = cp.Problem(objective, constraints)
    problem.solve()
    print(f"Status: {problem.status}")
    print(f"Optimal value (min cost): {problem.value:.2f}")
    print(f"x11 (Product 1): {x11.value:.2f}")
    print(f"x12 (Product 1): {x12.value:.2f}")
    print(f"x13 (Product 1): {x13.value:.2f}")
    print(f"x14 (Product 1): {x14.value:.2f}")
    print(f"x15 (Product 1): {x15.value:.2f}")
    print(f"x21 (Product 1): {x21.value:.2f}")
    print(f"x22 (Product 1): {x22.value:.2f}")
    print(f"x23 (Product 1): {x23.value:.2f}")
    print(f"x24 (Product 1): {x24.value:.2f}")
    print(f"x25 (Product 1): {x25.value:.2f}")
    print(f"x31 (Product 1): {x31.value:.2f}")
    print(f"x32 (Product 1): {x32.value:.2f}")
    print(f"x33 (Product 1): {x33.value:.2f}")
    print(f"x34 (Product 1): {x34.value:.2f}")
    print(f"x35 (Product 1): {x35.value:.2f}")
    print("Check constraint:")
    print(f"x11 + x12 + x13 + x14 + x15 = {(x11 + x12 + x13 + x14 + x15).value} <= 100")
    print(f"x21 + x22 + x23 + x24 + x25 = {(x21 + x22 + x23 + x24 + x25).value} <= 150")
    print(f"x31 + x32 + x33 + x34 + x35 = {(x31 + x32 + x33 + x34 + x35).value} <= 200")
    print(f"x11 + x21 + x31 = {(x11 + x21 + x31).value} == 80")
    print(f"x12 + x22 + x32 = {(x12 + x22 + x32).value} == 90")
    print(f"x13 + x23 + x33 = {(x13 + x23 + x33).value} == 110")
    print(f"x14 + x24 + x34 = {(x14 + x24 + x34).value} == 120")
    print(f"x15 + x25 + x35 = {(x15 + x25 + x35).value} == 50")