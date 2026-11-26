Hipótese: Com a rede a 2s, o circuito abre e o STIP aprova compras pequenas.

authorization
Warning
23:44:54,683
Resilience event occurred. EventName: 'OnRetry', Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The operation didn't complete within the allowed timeout of '00:00:00.3000000'.'
338e2f9
authorization
Information
23:44:54,744
Sending HTTP request POST https://localhost:7103/authorize
338e2f9
authorization
Error
23:44:55,046
Resilience event occurred. EventName: 'OnTimeout', Source: 'NetworkClient-card-network//Timeout', Operation Key: '', Result: ''
338e2f9
authorization
Warning
23:44:55,048
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The operation didn't complete within the allowed timeout of '00:00:00.3000000'.', Handled: 'True', Attempt: '1', Execution Time: 303,0341ms
338e2f9
authorization
Warning
23:44:55,049
Resilience event occurred. EventName: 'OnRetry', Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The operation didn't complete within the allowed timeout of '00:00:00.3000000'.'
338e2f9
authorization
Information
23:44:55,092
Sending HTTP request POST https://localhost:7103/authorize
338e2f9
authorization
Information
23:44:55,184
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The operation was canceled.', Handled: 'False', Attempt: '2', Execution Time: 92,5191ms
338e2f9
authorization
Error
23:44:55,185
Resilience event occurred. EventName: 'OnTimeout', Source: 'NetworkClient-card-network//Timeout', Operation Key: '', Result: ''
338e2f9
authorization
Warning
23:44:55,186
Rede de cartões indisponível: TimeoutRejectedException
338e2f9
authorization
Information
23:44:55,237
Start processing HTTP request POST https+http://network-sim/authorize
a524aba
authorization
Information
23:44:55,237
Sending HTTP request POST https://localhost:7103/authorize
a524aba
authorization
Error
23:44:55,542
Resilience event occurred. EventName: 'OnTimeout', Source: 'NetworkClient-card-network//Timeout', Operation Key: '', Result: ''
a524aba
authorization
Warning
23:44:55,543
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The operation didn't complete within the allowed timeout of '00:00:00.3000000'.', Handled: 'True', Attempt: '0', Execution Time: 305,2467ms
a524aba
authorization
Warning
23:44:55,544
Resilience event occurred. EventName: 'OnRetry', Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The operation didn't complete within the allowed timeout of '00:00:00.3000000'.'
a524aba
authorization
Information
23:44:55,603
Sending HTTP request POST https://localhost:7103/authorize
a524aba
authorization
Error
23:44:55,905
Resilience event occurred. EventName: 'OnTimeout', Source: 'NetworkClient-card-network//Timeout', Operation Key: '', Result: ''
a524aba
authorization
Error
23:44:55,909
Resilience event occurred. EventName: 'OnCircuitOpened', Source: 'NetworkClient-card-network//CircuitBreaker', Operation Key: '', Result: 'The operation didn't complete within the allowed timeout of '00:00:00.3000000'.'
a524aba
authorization
Warning
23:44:55,912
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The operation didn't complete within the allowed timeout of '00:00:00.3000000'.', Handled: 'True', Attempt: '1', Execution Time: 308,4369ms
a524aba
authorization
Warning
23:44:55,913
Resilience event occurred. EventName: 'OnRetry', Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The operation didn't complete within the allowed timeout of '00:00:00.3000000'.'
a524aba
authorization
Information
23:44:55,965
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '2', Execution Time: 0,743ms
a524aba
authorization
Warning
23:44:55,966
Rede de cartões indisponível: BrokenCircuitException
a524aba
authorization
Information
23:44:56,017
Start processing HTTP request POST https+http://network-sim/authorize
392a808
authorization
Information
23:44:56,018
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0153ms
392a808
authorization
Warning
23:44:56,018
Rede de cartões indisponível: BrokenCircuitException
392a808
authorization
Information
23:44:56,076
Start processing HTTP request POST https+http://network-sim/authorize
9dfbf56
authorization
Information
23:44:56,076
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0199ms
9dfbf56
authorization
Warning
23:44:56,077
Rede de cartões indisponível: BrokenCircuitException
9dfbf56
authorization
Information
23:44:56,132
Start processing HTTP request POST https+http://network-sim/authorize
fb81ea2
authorization
Information
23:44:56,132
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0164ms
fb81ea2
authorization
Warning
23:44:56,133
Rede de cartões indisponível: BrokenCircuitException
fb81ea2
authorization
Information
23:44:56,187
Start processing HTTP request POST https+http://network-sim/authorize
34947fb
authorization
Information
23:44:56,187
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0141ms
34947fb
authorization
Warning
23:44:56,188
Rede de cartões indisponível: BrokenCircuitException
34947fb
authorization
Information
23:44:56,239
Start processing HTTP request POST https+http://network-sim/authorize
717443d
authorization
Information
23:44:56,239
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0135ms
717443d
authorization
Warning
23:44:56,240
Rede de cartões indisponível: BrokenCircuitException
717443d
authorization
Information
23:44:56,292
Start processing HTTP request POST https+http://network-sim/authorize
a1db00b
authorization
Information
23:44:56,293
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0139ms
a1db00b
authorization
Warning
23:44:56,293
Rede de cartões indisponível: BrokenCircuitException
a1db00b
authorization
Information
23:44:56,352
Start processing HTTP request POST https+http://network-sim/authorize
f40e450
authorization
Information
23:44:56,353
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0175ms
f40e450
authorization
Warning
23:44:56,353
Rede de cartões indisponível: BrokenCircuitException
f40e450
authorization
Information
23:44:56,410
Start processing HTTP request POST https+http://network-sim/authorize
58402d7
authorization
Information
23:44:56,410
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0201ms
58402d7
authorization
Warning
23:44:56,411
Rede de cartões indisponível: BrokenCircuitException
58402d7
authorization
Information
23:44:56,467
Start processing HTTP request POST https+http://network-sim/authorize
552fe6b
authorization
Information
23:44:56,468
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0138ms
552fe6b
authorization
Warning
23:44:56,468
Rede de cartões indisponível: BrokenCircuitException
552fe6b
authorization
Information
23:44:56,524
Start processing HTTP request POST https+http://network-sim/authorize
c7919e6
authorization
Information
23:44:56,525
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0293ms
c7919e6
authorization
Warning
23:44:56,525
Rede de cartões indisponível: BrokenCircuitException
c7919e6
authorization
Information
23:44:56,582
Start processing HTTP request POST https+http://network-sim/authorize
067bb0f
authorization
Information
23:44:56,585
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,017ms
067bb0f
authorization
Warning
23:44:56,586
Rede de cartões indisponível: BrokenCircuitException
067bb0f
authorization
Information
23:44:56,648
Start processing HTTP request POST https+http://network-sim/authorize
931e6c6
authorization
Information
23:44:56,648
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0146ms
931e6c6
authorization
Warning
23:44:56,648
Rede de cartões indisponível: BrokenCircuitException
931e6c6
authorization
Information
23:44:56,707
Start processing HTTP request POST https+http://network-sim/authorize
2cd2081
authorization
Information
23:44:56,707
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0136ms
2cd2081
authorization
Warning
23:44:56,707
Rede de cartões indisponível: BrokenCircuitException
2cd2081
authorization
Information
23:44:56,765
Start processing HTTP request POST https+http://network-sim/authorize
a1798b3
authorization
Information
23:44:56,766
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0176ms
a1798b3
authorization
Warning
23:44:56,766
Rede de cartões indisponível: BrokenCircuitException
a1798b3
authorization
Information
23:44:56,822
Start processing HTTP request POST https+http://network-sim/authorize
0cc8b5a
authorization
Information
23:44:56,822
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0139ms
0cc8b5a
authorization
Warning
23:44:56,823
Rede de cartões indisponível: BrokenCircuitException
0cc8b5a
authorization
Information
23:44:56,880
Start processing HTTP request POST https+http://network-sim/authorize
7c5810f
authorization
Information
23:44:56,881
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0149ms
7c5810f
authorization
Warning
23:44:56,881
Rede de cartões indisponível: BrokenCircuitException
7c5810f
authorization
Information
23:44:56,936
Start processing HTTP request POST https+http://network-sim/authorize
e881c2a
authorization
Information
23:44:56,936
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0123ms
e881c2a
authorization
Warning
23:44:56,936
Rede de cartões indisponível: BrokenCircuitException
e881c2a
authorization
Information
23:44:56,989
Start processing HTTP request POST https+http://network-sim/authorize
628a6ad
authorization
Information
23:44:56,989
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0151ms
628a6ad
authorization
Warning
23:44:56,989
Rede de cartões indisponível: BrokenCircuitException
628a6ad
authorization
Information
23:44:57,052
Start processing HTTP request POST https+http://network-sim/authorize
8fea17c
authorization
Information
23:44:57,052
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0145ms
8fea17c
authorization
Warning
23:44:57,053
Rede de cartões indisponível: BrokenCircuitException
8fea17c
authorization
Information
23:44:57,108
Start processing HTTP request POST https+http://network-sim/authorize
b5cbee2
authorization
Information
23:44:57,108
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0167ms
b5cbee2
authorization
Warning
23:44:57,109
Rede de cartões indisponível: BrokenCircuitException
b5cbee2
authorization
Information
23:44:57,169
Start processing HTTP request POST https+http://network-sim/authorize
e845a65
authorization
Information
23:44:57,169
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0171ms
e845a65
authorization
Warning
23:44:57,170
Rede de cartões indisponível: BrokenCircuitException
e845a65
authorization
Information
23:44:57,228
Start processing HTTP request POST https+http://network-sim/authorize
c74f248
authorization
Information
23:44:57,228
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0148ms
c74f248
authorization
Warning
23:44:57,229
Rede de cartões indisponível: BrokenCircuitException
c74f248
authorization
Information
23:44:57,293
Start processing HTTP request POST https+http://network-sim/authorize
dd25c3b
authorization
Information
23:44:57,293
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0148ms
dd25c3b
authorization
Warning
23:44:57,294
Rede de cartões indisponível: BrokenCircuitException
dd25c3b
authorization
Information
23:44:57,352
Start processing HTTP request POST https+http://network-sim/authorize
e59e3cc
authorization
Information
23:44:57,353
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0129ms
e59e3cc
authorization
Warning
23:44:57,353
Rede de cartões indisponível: BrokenCircuitException
e59e3cc
authorization
Information
23:44:57,411
Start processing HTTP request POST https+http://network-sim/authorize
9e08f55
authorization
Information
23:44:57,411
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0121ms
9e08f55
authorization
Warning
23:44:57,411
Rede de cartões indisponível: BrokenCircuitException
9e08f55
authorization
Information
23:44:57,468
Start processing HTTP request POST https+http://network-sim/authorize
1ea5b70
authorization
Information
23:44:57,469
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0179ms
1ea5b70
authorization
Warning
23:44:57,469
Rede de cartões indisponível: BrokenCircuitException
1ea5b70
authorization
Information
23:44:57,531
Start processing HTTP request POST https+http://network-sim/authorize
5fd250a
authorization
Information
23:44:57,531
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0147ms
5fd250a
authorization
Warning
23:44:57,532
Rede de cartões indisponível: BrokenCircuitException
5fd250a
authorization
Information
23:44:57,589
Start processing HTTP request POST https+http://network-sim/authorize
e91924e
authorization
Information
23:44:57,590
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0146ms
e91924e
authorization
Warning
23:44:57,590
Rede de cartões indisponível: BrokenCircuitException
e91924e
authorization
Information
23:44:57,648
Start processing HTTP request POST https+http://network-sim/authorize
75f4cba
authorization
Information
23:44:57,648
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0249ms
75f4cba
authorization
Warning
23:44:57,648
Rede de cartões indisponível: BrokenCircuitException
75f4cba
authorization
Information
23:44:57,713
Start processing HTTP request POST https+http://network-sim/authorize
d78f739
authorization
Information
23:44:57,714
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0135ms
d78f739
authorization
Warning
23:44:57,714
Rede de cartões indisponível: BrokenCircuitException
d78f739
authorization
Information
23:44:57,776
Start processing HTTP request POST https+http://network-sim/authorize
811e5fc
authorization
Information
23:44:57,777
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0163ms
811e5fc
authorization
Warning
23:44:57,777
Rede de cartões indisponível: BrokenCircuitException
811e5fc
authorization
Information
23:44:57,843
Start processing HTTP request POST https+http://network-sim/authorize
ba89f52
authorization
Information
23:44:57,844
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0125ms
ba89f52
authorization
Warning
23:44:57,844
Rede de cartões indisponível: BrokenCircuitException
ba89f52
authorization
Information
23:44:57,899
Start processing HTTP request POST https+http://network-sim/authorize
ab9c0fc
authorization
Information
23:44:57,900
Execution attempt. Source: 'NetworkClient-card-network//Retry', Operation Key: '', Result: 'The circuit is now open and is not allowing calls.', Handled: 'False', Attempt: '0', Execution Time: 0,0162ms
ab9c0fc
authorization
Warning
23:44:57,900
Rede de cartões indisponível: BrokenCircuitException
ab9c0fc
