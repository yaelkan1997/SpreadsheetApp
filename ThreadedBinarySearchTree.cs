using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadedBinarySearchTree
{
    class Node
    {
        private int value;
        private Node left;
        private Node Right;

        public Node()
        {
            this.value = 0;
            this.left = null;
            this.Right = null;
        }

        public static Node CreateNewNode(int val)
        {
            Node n = new Node();
            n.value = val;
            return n;
        }
        public int getValue()
        {
            return this.value;
        }
        public Node getLeftNode()
        {
            return this.left;
        }
        public Node getRightNode()
        {
            return this.Right;
        }
        public void setLeftNode(Node n)
        {
            this.left = n;
        }
        public void setRightNode(Node n)
        {
            this.Right = n;
        }
        public void setValue(int val)
        {
            this.value = val;
        }
    }
    class ThreadedBinarySearchTree
    {
        Node root;
        private static object _lockObject = new object();

        public void add(int num)
        {
            lock (_lockObject)
            {
                Node newnode = Node.CreateNewNode(num);
                //Tree Traversal
                if (root == null)
                {
                    this.root = newnode;
                    return;
                }
                if (search(num))
                    return;
                if (num < root.getValue())
                {
                    LeftAdd(newnode, root);
                }
                else
                {
                    RightAdd(newnode, root);
                }
            }
        }   //add num to the tree, if it already exists, do nothing

        public void remove(int num) {
            if (search(num))
                {
                    lock (_lockObject)
                    {
                        this.root = RemoveHelper(root, num);
                    }
                }
        } // remove num from the tree, if it doesn't exists, do nothing
        private Node RemoveHelper(Node root, int value)
        {
            if (root == null)
                return root;
            //if key is less than the root node,recurse left subtree
            if (value < root.getValue())
                root.setLeftNode(RemoveHelper(root.getLeftNode(), value));
            // if key is more than the root node,recurse right subtree
            else if (value > root.getValue())
            {
                root.setRightNode(RemoveHelper(root.getRightNode(), value));
            }
            //else we found the key
            else
            {
                //case 1: Node to be deleted has no children
                if (root.getLeftNode() == null && root.getRightNode() == null)
                {
                    //update root to null
                    root = null;
                }
                //case 2 : node to be deleted has two children
                else if (root.getLeftNode() != null && root.getRightNode() != null)
                {
                    Node maxNode = FindMax(root.getRightNode());
                    //copy the value
                    root.setValue(maxNode.getValue());
                    root.setRightNode(RemoveHelper(root.getRightNode(), maxNode.getValue()));
                }
                //node to be deleted has one children
                else
                {
                    Node child = root.getRightNode() != null ? root.getRightNode() : root.getLeftNode();
                    root = child;
                }

            }
            return root;
        }
        private Node FindMax(Node node)
        {
            while (node.getLeftNode() != null)
            {
                node = node.getLeftNode();
            }
            return node;
        }
        public bool search(int num) 
        {
            Node nodePtr = this.root;
            if (nodePtr == null)
                return false;
            if (nodePtr.getValue() == num)
                return true;
            if (nodePtr.getValue() > num)
                return LeftSearch(num, nodePtr.getLeftNode());
            else
                return RightSearch(num, nodePtr.getRightNode());
        } // search num in the tree and return true/false accordingly

        public void clear() {
            lock (_lockObject)
            {
                ClearPostTraversal(this.root);
                this.root = null;
            }
        } // remove all items from tree
        public void ClearPostTraversal(Node n)
        {
            if (n != null)
            {
                ClearPostTraversal(n.getLeftNode());
                ClearPostTraversal(n.getRightNode());
                n.setLeftNode(null);
                n.setRightNode(null);
                n = null;
            }
        }
        public void print()
        {
            if(this.root != null)
                printInorder(this.root);
        } // print the values of three from the smallest to largest in comma delimited form. For example; -15,0,1,3,5,23,40,89,201. If the tree is empty do nothing. 
        void printInorder(Node node)
        {
            if (node == null)
                return;

            /* first recur on left child */
            printInorder(node.getLeftNode());

            /* then print the data of node */
            if (node != null)
            {
                Console.Write(node.getValue() + " ");
            }

            /* now recur on right child */
            printInorder(node.getRightNode());
        }
        private void LeftAdd(Node newnode, Node father)
        {
            if (father == null)
            {
                father = newnode;
                return;
            }
            if (father.getLeftNode() == null)
                father.setLeftNode(newnode);
            else
            {
                Node nodePtr = father.getLeftNode();
                if (newnode.getValue() < nodePtr.getValue())
                {
                    LeftAdd(newnode, nodePtr);
                }
                else
                {
                    RightAdd(newnode, nodePtr);
                }
            }
        }
        private void RightAdd(Node newnode, Node father)
        {
            if (father == null)
            {
                father = newnode;
                return;
            }
            if (father.getRightNode() == null)
                father.setRightNode(newnode);
            else
            {
                Node nodePtr = father.getRightNode();
                if (newnode.getValue() < nodePtr.getValue())
                    LeftAdd(newnode, nodePtr);
                else
                    RightAdd(newnode, nodePtr);
            }
        }
        private bool LeftSearch(int num, Node father)
        {
            if (father == null)
                return false;
            if (father.getValue() == num)
                return true;
            if (father.getValue() > num)
                return LeftSearch(num, father.getLeftNode());
            else
                return RightSearch(num, father.getRightNode());

        }
        private bool RightSearch(int num, Node father)
        {
            if (father == null)
                return false;
            if (father.getValue() == num)
                return true;
            if (father.getValue() > num)
                return LeftSearch(num, father.getLeftNode());
            else
                return RightSearch(num, father.getRightNode());
        }
       


    }
}
