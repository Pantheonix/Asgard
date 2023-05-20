terraform {
  required_version = ">= 0.12"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.0"
    }
  }

}

resource "aws_secretsmanager_secret" "pantheonix_secretsmanager" {
  for_each = var.secrets

  name = each.key

}

resource "aws_secretsmanager_secret_version" "pantheonix_secretsmanager" {
  for_each = var.secrets

  secret_id     = aws_secretsmanager_secret.pantheonix_secretsmanager[each.key].id
  secret_string = each.value

}
