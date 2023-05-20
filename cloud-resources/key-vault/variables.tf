variable "aws_region" {
  description = "AWS region"
  type        = string

}

variable "secrets" {
  description = "Map of secrets to be stored in AWS Secrets Manager"
  type        = map(string)

}

