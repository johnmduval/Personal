from contact_tracing import *

ct = ContactTracer("data/data4.csv")
print("Contact Records:")
print(ct)

bord = "-"*10+'\n'

print(bord,"Potential Zombies:\n")
zombies = ct.get_potential_zombies()
for zom in zombies:
  print(zom.label)
  

print(bord,"Patient Zeros:\n")
p_zeros = ct.get_patient_zeros()
for patient in p_zeros:
  print(patient.label)

print(bord,"Closest Patient-Zero - Zombie Connections:\n")

# Vertex Objects: Sarai, Philip 
for zom in zombies:
  print(f"=== zombie '{zom}' ===")
  if (zom.label != 'Smith'):
    continue
  closest_patient = None 
  closest_dist = float('inf')
  closest_path = None
  # Vertex Objects: Bob, Farley, Larry
  for patient in p_zeros:
    print(f"    === patient '{patient}' ===")
    if (patient.label != 'Al'):
      print('got here')
    ct.print_table()
    ct.dijkstra_shortest_path(patient)
    ct.print_table()
    if zom.distance < closest_dist:
      closest_dist = zom.distance
      closest_patient = patient
      closest_path = ct.print_shortest_path(closest_patient, zom)
      # print(f"distance: {closest_dist}\n closest zero: {closest_patient}")
    # print(f"zom: {zom} patient: {patient}\nPath: {ct.print_shortest_path(patient,zom)}")

  if closest_path != None:
    print(closest_path)


