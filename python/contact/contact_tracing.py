import csv

class ContactTracer():
  def __init__(self, filename=None):
    self.contacts = {}
    self.edge_weights = {}
    self.read_file(filename)
    
    
  def read_file(self,filename):
    with open(filename, mode='r') as file:
      csvFile = csv.reader(file)
      # each line in the csv is formatted
      # name, contact1, contact2, ...
      self.all_lines = []
      for line in csvFile:
        # remainder of values is names of all contacts
        for contact_label in line[1:]:
          #contact = self.get_vertex(contact_label)
          self.add_edge(line[0],contact_label)
        self.all_lines.append(line)
      
        

  def get_vertex(self, vertex_label):
    '''
      Return the Vertex object associated with a label. If the label has no vertex, create one.

    Attributes:
      self; Graph.
      vertex_label; string; uniquie ID/label of vertex.
    Return:
      Vertex object with the label.
    '''
    for vertex in self.contacts.keys():
      if vertex.label == vertex_label:
        return vertex
    new_vtx = Vertex(vertex_label)
    self.contacts[new_vtx] = []
    return new_vtx

  def get_edge_weight(self, from_vtx, to_vtx):
    '''
    Return the edge weight between two verticies
    Parameters:
      from_vtx; Vertex.
      to_vtx; Vertex.
    '''
    return self.edge_weights[(from_vtx,to_vtx)]
    
  def add_edge(self, from_vtx_label, to_vtx_label, weight=1):
    '''
    Add edge of given weight (default 1) between to verticies.
    Parameters:
      from_vtx_label; string.
      to_vtx_label; string.
    Return: None
    '''
    # get or create Vertex objects for each label
    fm_vtx = self.get_vertex(from_vtx_label) 
    to_vtx = self.get_vertex(to_vtx_label)

    # add to_vertex to from vertex's adjacency list
    self.contacts[fm_vtx].append(to_vtx)
    # set eight of that edge
    self.edge_weights[(fm_vtx, to_vtx)] = weight
    
  def __str__(self):
    '''
    Return string representing all verticies and their adjacent verticies.
    Paramseters: self.
    Return: string.
    '''
    s = "\n"
    for line in self.all_lines:
      sick = line[0]
      contacts = ""
      for i in range(1, len(line)-1):
        new_contact = line[i]
        contacts += (new_contact + ", ")
      if len(line) > 2:
        for i in range(len(line)-1, len(line)):
          new_contact = line[i]
          contacts = contacts.rstrip(", ")
          contacts += (" and " + new_contact)
      else:
        for i in range(1, len(line)):
          new_contact = line[i]
          contacts += (new_contact)
      
      s += (f"{sick} had contact with {contacts}")
      # https://docs.python.org/2/library/string.html#string.rstrip
      s = s.rstrip(", ")
      s += "\n"

      
    return s

  def get_potential_zombies(self):
    '''
    Return a list of Vertex objects of all potential zombies, i.e., vertices with no outgoing edges.
    Paramteres: self.
    Return: listof Vertex objects.
    '''
    zombies = []
    all_people = []
    first_people = []

    
    for line in self.all_lines:
      for person in line:
        all_people.append(person)
      first_people.append(line[0])
    
    for person in all_people:
      if person not in first_people:
        zombies.append(self.get_vertex(person))
    
    no_dups = []
    for potential in zombies:
      if potential not in no_dups:
        no_dups.append(potential)

        
    return no_dups
  
  def get_patient_zeros(self):
    '''
    Returns list of Vertex objects of the patient zeros, i.e., verticies with no incoming edges.
    
    Paramters: self.
    Return: list of Vertex objects.
    '''
    zeroes = []
    infected_list = []
    not_first = []
    
    for person in self.contacts:
      infected_list.append(person)
      contact_list = self.contacts[person]
      for contact in contact_list:
        not_first.append(contact)
    for person in infected_list:
      if person not in not_first:
        zeroes.append(person)
    
    return zeroes

  def print_table(self):
    for c in self.contacts:
      print(f"{c.label}: {c.distance}")

  def dijkstra_shortest_path(self, start_vertex):
    '''
      Runs Dijkstra's shortest path algorithm to find shortest path from start_vertex_label to all
      other connected vertices.
      Prints all shortest distances.
    Parameters:
      start_vertex_label: label of vertex in graph. if label does not exist, does nothing. 
    Return: None
    '''
    # check if the key is valid
    if start_vertex not in self.contacts:
      return

    # set each vertex distance to inf & prev to None
    for vertex in self.contacts.keys():
      vertex.distance = float("inf")
      vertex.prev_vertex = None

      
    # create deep copy of all vertices to start them in unvisited list
    unvisited = list(self.contacts.keys())[:]

    #print(f"starting person: {start_vertex}")
    start_vertex.distance = 0

    # while there are still nodes left to visit
    while len(unvisited) > 0:
      # set current vertex to vertex with the shortest distance in unvisited
      min = float("inf")
      for vertex in unvisited:
        if vertex.distance < min:
          min = vertex.distance
          curr = vertex
          #print(f"curr1: {curr}")
          #print("breakpoint")
 
      if min == float("inf"):
        break
        
      # find neighbors of curr
      unvisited_neighbors = []
      for vertex in unvisited:
        # print(f"start vertex: {start_vertex} vertex: {vertex}")
        existing_edges = self.edge_weights.keys()
        if (curr, vertex) in existing_edges:
          unvisited_neighbors.append(vertex)

      # for each neighbor, calculate a distance
      for neighbor in unvisited_neighbors:
        if (curr, neighbor) in self.edge_weights:
          edge_weight = self.edge_weights[(curr, neighbor)]
          possible_dist = curr.distance + edge_weight

          #print(f"neighbor: {neighbor}")
          #print(f"neighbor dist: {neighbor.distance} possible dist: {possible_dist}")
          if possible_dist < neighbor.distance:
            
            neighbor.distance = possible_dist
            #print(f"updated neighbor dist: {neighbor.distance}")
            neighbor.prev_vertex = curr

      #print(f"removing: {curr}")
      unvisited.remove(curr)
      #print(len(unvisited))
      #print("breakpoint")

    for vertex in self.contacts:
      pass
      #print(f"{vertex.label} distance: {vertex.distance}")



      
    
  
  def print_shortest_path(self, start_vertex, end_vertex):
    '''
    Print shortest path and length of shortest path between twoVerticies.
    Parameters:
      start_vertex: Vertex --> closest zero
      end_vertex: Vertex --> contact
    Return:
    None
    '''
    # self.dijkstra_shortest_path(start_vertex)
    if end_vertex.label == "Smith":
      print("here")
    
    path = ""
    current_vertex = end_vertex
    # end vertex stores distance
    dist = current_vertex.distance

    while current_vertex != start_vertex and current_vertex != None:
      path = " -> " + str(current_vertex.label) + path
      current_vertex = current_vertex.prev_vertex
      
    path = start_vertex.label + path + ". Dist: "+ str(dist) +'\n'
    return path
    

class Vertex:
  def __init__(self, label):
    self.label = label
    self.distance = float('inf')
    self.prev_vertex = None

  def __str__(self):
    return self.label
