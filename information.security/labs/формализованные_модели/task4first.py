import cvxpy as cp

if __name__ == "__main__":
    z1 = cp.Variable(nonneg=True)
    z2 = cp.Variable(nonneg=True)
    z3 = cp.Variable(nonneg=True)
    objective = cp.Minimize(z1 + z2 + z3)
    constraints = [
        2 * z1 + 6 * z2 + 3 * z3 >= 1,
        4 * z1 + 2 * z2 + 2 * z3 >= 1,
        8 * z1 + 4 * z2 + 5 * z3 >= 1,
        5 * z1 + 6 * z2 + 4 * z3 >= 1
    ]
    problem = cp.Problem(objective, constraints)
    problem.solve()
    print(f"Status: {problem.status}")
    print(f"Optimal value: {problem.value:.2f}")
    print(f"z1: {z1.value:.2f}")
    print(f"z2: {z2.value:.2f}")
    print(f"z3: {z3.value:.2f}")
    print("Check constraint:")
    print(f"2 * z1 + 6 * z2 + 3 * z3 = {(2 * z1 + 6 * z2 + 3 * z3).value} >= 1")
    print(f"4 * z1 + 2 * z2 + 2 * z3 = {(4 * z1 + 2 * z2 + 2 * z3).value} >= 1")
    print(f"8 * z1 + 4 * z2 + 5 * z3 = {(8 * z1 + 4 * z2 + 5 * z3).value} >= 1")
    print(f"5 * z1 + 6 * z2 + 4 * z3 = {(5 * z1 + 6 * z2 + 4 * z3).value} >= 1")
    v = 1 / problem.value
    print("First player result :")
    print(f"v = {v}")
    print(f"x1 = {v * z1.value}")
    print(f"x2 = {v * z2.value}")
    print(f"x3 = {v * z3.value}")