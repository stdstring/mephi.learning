import cvxpy as cp

if __name__ == "__main__":
    z1 = cp.Variable(nonneg=True)
    z2 = cp.Variable(nonneg=True)
    z3 = cp.Variable(nonneg=True)
    z4 = cp.Variable(nonneg=True)
    objective = cp.Minimize(z1 + z2 + z3 + z4)
    constraints = [
        8 * z1 + 3 * z2 + 2 * z3 + 6 * z4 >= 1,
        7 * z1 + 9 * z2 + 3 * z3 + 5 * z4 >= 1,
        1 * z1 - 1 * z2 - 4 * z3 + 7 * z4 >= 1,
        7 * z1 + 1 * z2 + 1 * z3 + 6 * z4 >= 1
    ]
    problem = cp.Problem(objective, constraints)
    problem.solve()
    print(f"Status: {problem.status}")
    print(f"Optimal value: {problem.value:.2f}")
    print(f"z1: {z1.value:.2f}")
    print(f"z2: {z2.value:.2f}")
    print(f"z3: {z3.value:.2f}")
    print(f"z4: {z4.value:.2f}")
    print("Check constraint:")
    print(f"8 * z1 + 3 * z2 + 2 * z3 + 6 * z4 = {(8 * z1 + 3 * z2 + 2 * z3 + 6 * z4).value} >= 1")
    print(f"7 * z1 + 9 * z2 + 3 * z3 + 5 * z4 = {(7 * z1 + 9 * z2 + 3 * z3 + 5 * z4).value} >= 1")
    print(f"1 * z1 - 1 * z2 - 4 * z3 + 7 * z4 = {(1 * z1 - 1 * z2 - 4 * z3 + 7 * z4).value} >= 1")
    print(f"7 * z1 + 1 * z2 + 1 * z3 + 6 * z4 = {(7 * z1 + 1 * z2 + 1 * z3 + 6 * z4).value} >= 1")
    v = 1 / problem.value
    print(f"game score: {v:.2f}")
    print(f"x1: {z1.value * v:.2f}")
    print(f"x2: {z2.value * v:.2f}")
    print(f"x3: {z3.value * v:.2f}")
    print(f"x4: {z4.value * v:.2f}")