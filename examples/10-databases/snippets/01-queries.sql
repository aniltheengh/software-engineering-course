-- === SQL: schema, data and queries ===
CREATE TABLE customers (id INTEGER PRIMARY KEY, name TEXT NOT NULL, city TEXT NOT NULL);
CREATE TABLE products  (id INTEGER PRIMARY KEY, name TEXT NOT NULL, category TEXT NOT NULL, price NUMERIC NOT NULL);
CREATE TABLE orders    (id INTEGER PRIMARY KEY, customer_id INTEGER NOT NULL REFERENCES customers(id), ordered_at DATE NOT NULL);
CREATE TABLE order_items (
  order_id   INTEGER NOT NULL REFERENCES orders(id),
  product_id INTEGER NOT NULL REFERENCES products(id),
  quantity   INTEGER NOT NULL CHECK (quantity > 0),
  PRIMARY KEY (order_id, product_id)
);

INSERT INTO customers VALUES (1, 'Aiko', 'Tokyo'), (2, 'Ben', 'Osaka'), (3, 'Chandra', 'Tokyo');
INSERT INTO products  VALUES (1, 'Ball valve', 'Valves', 40), (2, 'Gate valve', 'Valves', 55), (3, 'Gasket', 'Seals', 3);
INSERT INTO orders    VALUES (100, 1, '2026-01-05'), (101, 1, '2026-02-09'), (102, 2, '2026-02-11');
INSERT INTO order_items VALUES (100, 1, 2), (100, 3, 10), (101, 2, 1), (102, 3, 50);

-- Filter and sort
SELECT name FROM customers WHERE city = 'Tokyo' ORDER BY name;

-- Join and aggregate: total per order            -> 100: 110, 101: 55, 102: 150
SELECT o.id, c.name, SUM(i.quantity * p.price) AS total
FROM orders o
JOIN customers c   ON c.id = o.customer_id
JOIN order_items i ON i.order_id = o.id
JOIN products p    ON p.id = i.product_id
GROUP BY o.id, c.name
ORDER BY o.id;

-- Group and filter the groups                    -> Seals: 180
SELECT p.category, SUM(i.quantity * p.price) AS revenue
FROM order_items i JOIN products p ON p.id = i.product_id
GROUP BY p.category
HAVING SUM(i.quantity * p.price) > 150;

-- Outer join: customers with no orders           -> Chandra
SELECT c.name
FROM customers c LEFT JOIN orders o ON o.customer_id = c.id
WHERE o.id IS NULL;

-- Window function: rank customers by spend       -> Aiko 1, Ben 2
SELECT c.name,
       SUM(i.quantity * p.price) AS spend,
       RANK() OVER (ORDER BY SUM(i.quantity * p.price) DESC) AS position
FROM customers c
JOIN orders o      ON o.customer_id = c.id
JOIN order_items i ON i.order_id = o.id
JOIN products p    ON p.id = i.product_id
GROUP BY c.id, c.name;
