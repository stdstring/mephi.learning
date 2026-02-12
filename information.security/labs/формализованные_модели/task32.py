import cvxpy as cp

if __name__ == "__main__":
    x1 = cp.Variable(integer=True, nonneg=True)
    x2 = cp.Variable(integer=True, nonneg=True)
    x3 = cp.Variable(integer=True, nonneg=True)
    objective = cp.Minimize(3 * x1 + 5 * x2 + 8 * x3)
    constraints = [
        5 * x1 + 10 * x2 + 15 * x3 >= 50,
        2 * x1 + 5 * x2 + 7 * x3 >= 30,
        10 * x1 + 20 * x2 + 25 * x3 >= 100
    ]
    problem = cp.Problem(objective, constraints)
    problem.solve()
    print(f"Status: {problem.status}")
    print(f"Optimal value (min cost): {problem.value:.2f}")
    print(f"x1 (Product 1): {x1.value:.2f}")
    print(f"x2 (Product 2): {x2.value:.2f}")
    print(f"x2 (Product 3): {x3.value:.2f}")
    print("Check constraint:")
    print(f"5 * x1 + 10 * x2 + 15 * x3 = {(5 * x1 + 10 * x2 + 15 * x3).value} >= 50")
    print(f"2 * x1 + 5 * x2 + 7 * x3 = {(2 * x1 + 5 * x2 + 7 * x3).value} >= 30")
    print(f"10 * x1 + 20 * x2 + 25 * x3 = {(10 * x1 + 20 * x2 + 25 * x3).value} >= 100")