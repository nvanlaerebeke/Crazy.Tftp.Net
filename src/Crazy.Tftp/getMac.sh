#!/bin/bash
arp -n "$IP" | sed -n '2 p' | awk '{print $3}' | tr --delete '\n'