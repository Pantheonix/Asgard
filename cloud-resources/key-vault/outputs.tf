output "secret_name_and_arn" {
  value = {
    for name, secret in aws_secretsmanager_secret.pantheonix_secretsmanager :
    name => secret.arn
  }
}
