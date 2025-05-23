import asyncio


async def exchange_with_server(reader: asyncio.StreamReader, writer: asyncio.StreamWriter, source_data: bytes) -> bytes:
    writer.write(f"{source_data.hex()}\n".encode())
    await writer.drain()
    data = await reader.readline()
    return bytes.fromhex(data.decode()[:-1])


async def solve():
    reader, writer = await asyncio.open_connection("crypto.mephictf.ru", 9131)
    print(await reader.readline())
    print(await reader.readline())
    # 3 * 16 = 48
    flag_length = 36
    portion_size = 3 * 16
    result = bytes()
    for index in range(flag_length):
        source_prefix = (portion_size - 1 - index) * b"\x00"
        actual_data = await exchange_with_server(reader, writer, source_prefix)
        found = False
        for number in range(256):
            expected_prefix = source_prefix + result + bytes([number])
            expected_data = await exchange_with_server(reader, writer, expected_prefix)
            if actual_data[portion_size - 1 - index:portion_size] == expected_data[portion_size - 1 - index:portion_size]:
                result += bytes([number])
                found = True
                break
        if not found:
            raise logic_error("Bad data")
        print(f"Found: {result.decode()}")
    writer.close()
    await writer.wait_closed()
    print(f"Result: {result.decode()}")


asyncio.run(solve())